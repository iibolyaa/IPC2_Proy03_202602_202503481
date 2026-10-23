
using System;
using System.Globalization;
using Backend.Modelos;
using Backend.Validaciones;

namespace Backend.Ayuda
{
    public class GestorAutorizaciones
    {
        private readonly EstadoSistema estado;
        private readonly ValidadorFecha validadorFecha;
        private readonly ValidadorDTE validadorDTE;
        private readonly GestorCorrelativos gestorCorrelativos;

        public GestorAutorizaciones(EstadoSistema estado)
        {
            this.estado = estado;

            validadorFecha = new ValidadorFecha();
            validadorDTE = new ValidadorDTE();

            gestorCorrelativos = new GestorCorrelativos(estado.Correlativos);
        }

        // Registrar un error de solicitud.
        private void RegistrarError(DateTime fecha, string referencia, string tipo, string descripcion)
        {
            ErrorFactura error = new ErrorFactura(fecha, referencia, tipo, descripcion);

            estado.Errores.Insertar(error);
        }

        private void RegistrarSolicitud(DateTime fechaRecepcion, DateTime? fechaDTE, string referencia,
            bool aprobada)
        {
            SolicitudProcesada registro = new SolicitudProcesada(fechaRecepcion, fechaDTE,
                    referencia, aprobada);

                estado.Solicitudes.Insertar(registro);
        }

        // Procesar una solicitud individual.
        public bool ProcesarSolicitud(SolicitudDTE solicitud, ResultadoAutorizaciones resultado,
            DateTime fechaAutorizacion)
        {
            bool aprobado = false;

            // Toda modificacion y guardado se realizabajo el bloqueo compartido del sistema.
            estado.EjecutarCambio(() =>
            {
                aprobado = ProcesarInternamente(solicitud, resultado, fechaAutorizacion);
            });

            return aprobado;
        }

        // Logica interna de autorizacion.
        private bool ProcesarInternamente(SolicitudDTE solicitud, ResultadoAutorizaciones resultado,
            DateTime fechaAutorizacion)
        {
            string referencia = solicitud.Referencia ?? "";

            // 1. Verificar la fecha del DTE
            bool fechaCorrecta = validadorFecha.IntentarObtenerFecha(solicitud.Tiempo, out DateTime fechaDTE);

            if (!fechaCorrecta)
            {
                RegistrarError(fechaAutorizacion, referencia, "FECHA_INVALIDA",
                    "No se encontro una fecha y hora valida.");

                RegistrarSolicitud(fechaAutorizacion, null, referencia, false);
                
                return false;
            }

            // 2. Verificar que existan los montos
            if (!solicitud.Valor.HasValue || !solicitud.Iva.HasValue || !solicitud.Total.HasValue)
            {
                RegistrarError(fechaDTE, referencia, "DATOS_INCOMPLETOS", "Faltan VALOR, IVA o TOTAL.");

                RegistrarSolicitud(fechaAutorizacion, fechaDTE, referencia, false);

                return false;
            }

            // Verificar referencias utilizadas anteriormente.
            if (!string.IsNullOrWhiteSpace(referencia) && estado.Solicitudes.ExisteReferencia(referencia))
            {
                RegistrarError(fechaDTE, referencia, "REFERENCIA_DUPLICADA", "La referencia ya fue procesada anteriormente.");

                RegistrarSolicitud(fechaAutorizacion, fechaDTE, referencia, false);

                return false;
            }

            // 3. Crear el DTE
            Factura documento = new Factura(fechaDTE, referencia,
                solicitud.NitEmisor ?? "", solicitud.NitReceptor ?? "",
                solicitud.Valor.Value, solicitud.Iva.Value, solicitud.Total.Value
            );

            // 4. Validar el contenido del documento
            bool valido = validadorDTE.Validar(documento, estado.Contribuyentes,
                estado.Documentos, estado.Errores);

            if (!valido)
            {
                // Contabilizar NIT inexistentes.
                ValidadorNit validadorNit = new ValidadorNit();

                if (validadorNit.Validar(documento.NitEmisor) && estado.Contribuyentes.Buscar(
                        documento.NitEmisor) == null)
                {
                    resultado.CantidadAutorizacionesRechazadasPorNitEmisorInexistente++;
                }

                if (validadorNit.Validar(documento.NitReceptor) && estado.Contribuyentes.Buscar(
                        documento.NitReceptor) == null)
                {
                    resultado.CantidadAutorizacionesRechazadasPorNitReceptorInexistente++;
                }

                // Conservar el DTE rechazado siempre que tenga una referencia nueva y no vacia.
                if (!string.IsNullOrWhiteSpace(referencia) &&
                    estado.Documentos.BuscarPorReferencia(referencia) == null)
                {
                    estado.Documentos.Insertar(documento);
                }

                RegistrarSolicitud(fechaAutorizacion, fechaDTE, referencia, false);

                return false;
            }

            // 5. Generar el codigo de autorizacion
            string codigo = gestorCorrelativos.GenerarCodigo(fechaAutorizacion);

            // 6. Marcar el DTE como autorizado
            documento.CodigoAutorizacion = codigo;
            documento.Autorizada = true;

            // 7. Almacenar el documento
            bool insertado = estado.Documentos.Insertar(documento);

            if (!insertado)
            {
                throw new InvalidOperationException("No se pudo registrar el DTE autorizado.");
            }

            // 8. Agregar el detalle de aprobacion
            long codigoNumerico = long.Parse(codigo, CultureInfo.InvariantCulture);

            resultado.AgregarAprobacion(referencia,codigoNumerico);

            RegistrarSolicitud(fechaAutorizacion, fechaDTE, referencia, true);

            return true;
        }
    }
}
