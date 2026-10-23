
using System;
using Backend.Modelos;
using Backend.Estructuras;

namespace Backend.Validaciones
{
    public class ValidadorDTE
    {
        private readonly ValidadorNit validadorNit;

        public ValidadorDTE()
        {
            validadorNit = new ValidadorNit();
        }

        // Registrar un error encontrado.
        private void RegistrarError(ListaErrores errores, Factura dte, string tipo,
            string descripcion)
        {
            ErrorFactura error = new ErrorFactura(dte.Fecha, dte.Referencia,
                tipo, descripcion);

            errores.Insertar(error);
        }

        // Verificar que el valor sea positivo y tenga como maximo dos decimales
        private bool ValidarMonto(decimal monto)
        {
            if (monto <= 0)
            {
                return false;
            }

            return decimal.Round(monto, 2) == monto;
        }

        // Calcular el IVA correspondiente al 12%.
        public decimal CalcularIva(decimal valor)
        {
            return decimal.Round(valor * 0.12m, 2, MidpointRounding.AwayFromZero);
        }

        // Validar un DTE y registrar sus errores.
        public bool Validar(Factura dte, ListaContribuyentes contribuyentes, ListaFacturas documentos,
            ListaErrores errores)
        {
            bool esValido = true;

            // 1. Validar referencia.
            if (string.IsNullOrWhiteSpace(dte.Referencia) ||
                dte.Referencia.Length > 40)
            {
                RegistrarError(errores, dte, 
                "REFERENCIA_INVALIDA", "La referencia esta vacia o supera 40 caracteres.");

                esValido = false;
            }
            else if (documentos.BuscarPorReferencia(dte.Referencia) != null)
            {
                RegistrarError(errores, dte, "REFERENCIA_DUPLICADA", "La referencia ya fue registrada.");

                esValido = false;
            }

            // 2. Validar NIT emisor.
            if (!validadorNit.Validar(dte.NitEmisor))
            {
                RegistrarError(errores, dte, "NIT_EMISOR_INVALIDO", "El NIT emisor no es valido.");

                esValido = false;
            }
            else if (contribuyentes.Buscar(dte.NitEmisor) == null)
            {
                RegistrarError(errores, dte, "NIT_EMISOR_INEXISTENTE", 
                "El NIT emisor no existe en el RTU.");

                esValido = false;
            }

            // 3. Validar NIT receptor.
            if (!validadorNit.Validar(dte.NitReceptor))
            {
                RegistrarError(errores, dte, "NIT_RECEPTOR_INVALIDO", "El NIT receptor no es valido.");

                esValido = false;
            }
            else if (contribuyentes.Buscar(dte.NitReceptor) == null)
            {
                RegistrarError(errores, dte, "NIT_RECEPTOR_INEXISTENTE", 
                "El NIT receptor no existe en el RTU.");

                esValido = false;
            }

            // 4. Validar el valor del DTE.
            if (!ValidarMonto(dte.Valor))
            {
                RegistrarError(errores, dte, "VALOR_INVALIDO",
                "El valor debe ser positivo y tener hasta dos decimales.");

                esValido = false;
            }

            // 5. Validar el IVA.
            decimal ivaEsperado = CalcularIva(dte.Valor);

            if (dte.Iva != ivaEsperado)
            {
                RegistrarError(errores, dte, "IVA_INCORRECTO",
                    "El IVA no corresponde al 12% del valor.");

                esValido = false;
            }

            // 6. Validar el total.
            decimal totalEsperado = dte.Valor + dte.Iva;

            if (dte.Total != totalEsperado)
            {
                RegistrarError(errores, dte, "TOTAL_INCORRECTO",
                    "El total no corresponde a VALOR + IVA.");

                esValido = false;
            }

            return esValido;
        }
    }
}
