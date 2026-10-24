
using System;
using Backend.Estructuras;
using Backend.Modelos;

namespace Backend.Ayuda
{
    public class GestorConsultas
    {
        private readonly EstadoSistema estado;

        public GestorConsultas(EstadoSistema estado)
        {
            this.estado = estado;
        }

        // Consultar documentos emitidos o recibidos
        // por un contribuyente en un rango de fechas.
        public ListaResumenContribuyente ConsultarContribuyente(string nit, DateTime fechaInicio, DateTime fechaFin,
            bool emitidas)
        {
            return estado.Consultar(() =>
            {
                ListaResumenContribuyente resumen = new ListaResumenContribuyente();

                for (int i = 0; i < estado.Documentos.ObtenerCantidad(); i++)
                {
                    Factura? dte = estado.Documentos.ObtenerPorIndice(i);

                    if (dte == null || !dte.Autorizada)
                    {
                        continue;
                    }

                    if (dte.Fecha.Date < fechaInicio.Date || dte.Fecha.Date > fechaFin.Date)
                    {
                        continue;
                    }

                    bool coincide = emitidas ? dte.NitEmisor == nit: dte.NitReceptor == nit;

                    if (coincide)
                    {
                        resumen.AgregarMovimiento(dte.Fecha, dte.Total);
                    }
                }

                return resumen;
            });
        }

        // Consultar el IVA emitido y recibido
        // por todos los NIT en una fecha.
        public ListaResumenIva ConsultarIva(DateTime fecha)
        {
            return estado.Consultar(() =>
            {
                ListaResumenIva resumen = new ListaResumenIva();

                for (int i = 0; i < estado.Documentos.ObtenerCantidad(); i++)
                {
                    Factura? dte = estado.Documentos.ObtenerPorIndice(i);

                    if (dte == null || !dte.Autorizada)
                    {
                        continue;
                    }

                    if (dte.Fecha.Date != fecha.Date)
                    {
                        continue;
                    }

                    resumen.AgregarEmitido(dte.NitEmisor, dte.Iva);

                    resumen.AgregarRecibido(dte.NitReceptor, dte.Iva);
                }

                return resumen;
            });
        }

        // Consultar documentos autorizados
        // agrupados por fecha.
        public ListaResumenFecha ConsultarRango(DateTime fechaInicio, DateTime fechaFin)
        {
            return estado.Consultar(() =>
            {
                ListaResumenFecha resumen = new ListaResumenFecha();

                for (int i = 0; i < estado.Documentos.ObtenerCantidad(); i++)
                {
                    Factura? dte = estado.Documentos.ObtenerPorIndice(i);

                    if (dte == null || !dte.Autorizada)
                    {
                        continue;
                    }

                    if (dte.Fecha.Date < fechaInicio.Date || dte.Fecha.Date > fechaFin.Date)
                    {
                        continue;
                    }

                    resumen.AgregarMovimiento(dte.Fecha, dte.Valor, dte.Total);
                }

                return resumen;
            });
        }
    }
}
