
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class ReportePdfModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty(SupportsGet = true)]
        public string Clase { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string Nit { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string TipoDocumento { get; set; } = "emitidas";

        [BindProperty(SupportsGet = true)]
        public string Inicio { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string Fin { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string Fecha { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string TipoMonto { get; set; } = "total";

        public string? MensajeError { get; private set; }

        public string RespuestaJson { get; private set; } = "";

        public bool MostrarReporte { get; private set; }

        public int CantidadDTE { get; private set; }

        public decimal MontoTotal { get; private set; }

        public decimal TotalIvaEmitido { get; private set; }

        public decimal TotalIvaRecibido { get; private set; }

        public string TituloReporte { get; private set; } = "";

        public string DescripcionReporte { get; private set; } = "";

        public string FechaGeneracion =>
            DateTime.Now.ToString(
                "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture
            );

        public ReportePdfModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        private bool ValidarFecha(string texto)
        {
            return DateTime.TryParseExact(
                texto,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }

        private bool ValidarRango()
        {
            if (!DateTime.TryParseExact(
                Inicio,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fechaInicio))
            {
                return false;
            }

            if (!DateTime.TryParseExact(
                Fin,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fechaFin))
            {
                return false;
            }

            return fechaInicio.Date <= fechaFin.Date;
        }

        public async Task OnGetAsync()
        {
            try
            {
                if (Clase == "contribuyente")
                {
                    if (string.IsNullOrWhiteSpace(Nit))
                    {
                        MensajeError = "Debe indicar un NIT.";
                        return;
                    }

                    if (!ValidarRango())
                    {
                        MensajeError =
                            "El rango de fechas no es valido.";
                        return;
                    }

                    if (TipoDocumento != "emitidas" &&
                        TipoDocumento != "recibidas")
                    {
                        MensajeError =
                            "El tipo de documento no es valido.";
                        return;
                    }

                    TituloReporte =
                        "Reporte de documentos por contribuyente";

                    DescripcionReporte =
                        $"NIT: {Nit} | " +
                        $"Tipo: {TipoDocumento} | " +
                        $"Periodo: {Inicio} al {Fin}";

                    RespuestaJson =
                        await clienteApi.ConsultarContribuyente(
                            Nit,
                            Inicio,
                            Fin,
                            TipoDocumento
                        );
                }
                else if (Clase == "iva")
                {
                    if (!ValidarFecha(Fecha))
                    {
                        MensajeError =
                            "La fecha de consulta no es valida.";
                        return;
                    }

                    TituloReporte = "Reporte de resumen de IVA";

                    DescripcionReporte =
                        $"Fecha consultada: {Fecha}";

                    RespuestaJson =
                        await clienteApi.ConsultarIva(Fecha);
                }
                else if (Clase == "rango")
                {
                    if (!ValidarRango())
                    {
                        MensajeError =
                            "El rango de fechas no es valido.";
                        return;
                    }

                    if (TipoMonto != "total" &&
                        TipoMonto != "siniva")
                    {
                        MensajeError =
                            "El tipo de monto no es valido.";
                        return;
                    }

                    bool incluirIva = TipoMonto == "total";

                    TituloReporte =
                        "Reporte de montos autorizados";

                    DescripcionReporte =
                        $"Periodo: {Inicio} al {Fin} | " +
                        (incluirIva ? "Con IVA" : "Sin IVA");

                    RespuestaJson =
                        await clienteApi.ConsultarRango(
                            Inicio,
                            Fin,
                            incluirIva
                        );
                }
                else
                {
                    MensajeError =
                        "El tipo de reporte solicitado no existe.";
                    return;
                }

                // Comprobacion del JSON recibido.
                using JsonDocument documento =
                    JsonDocument.Parse(RespuestaJson);

                JsonElement raiz = documento.RootElement;

                JsonElement detalle =
                    raiz.GetProperty("detalle");

                if (detalle.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        "El detalle no es un arreglo."
                    );
                }

                if (Clase == "contribuyente")
                {
                    CantidadDTE = raiz
                        .GetProperty("cantidadTotal")
                        .GetInt32();

                    MontoTotal = raiz
                        .GetProperty("montoTotal")
                        .GetDecimal();
                }
                else if (Clase == "iva")
                {
                    foreach (JsonElement registro in
                        detalle.EnumerateArray())
                    {
                        TotalIvaEmitido += registro
                            .GetProperty("ivaEmitido")
                            .GetDecimal();

                        TotalIvaRecibido += registro
                            .GetProperty("ivaRecibido")
                            .GetDecimal();
                    }
                }
                else
                {
                    MontoTotal = raiz
                        .GetProperty("montoTotal")
                        .GetDecimal();

                    foreach (JsonElement registro in
                        detalle.EnumerateArray())
                    {
                        CantidadDTE += registro
                            .GetProperty("cantidadDTE")
                            .GetInt32();
                    }
                }

                MostrarReporte = true;
            }
            catch (HttpRequestException ex)
            {
                MensajeError =
                    "No se pudo conectar con la API: " +
                    ex.Message;
            }
            catch (TaskCanceledException)
            {
                MensajeError =
                    "La consulta excedio el tiempo de espera.";
            }
            catch (JsonException)
            {
                MensajeError =
                    "La respuesta del Backend no tiene el formato esperado.";
            }
            catch (InvalidOperationException)
            {
                MensajeError =
                    "Los datos del reporte tienen un formato incorrecto.";
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                MensajeError =
                    "Faltan campos necesarios en la respuesta del Backend.";
            }
        }
    }
}
