using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class ResumenRangoModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty]
        public DateTime? FechaInicio { get; set; }

        [BindProperty]
        public DateTime? FechaFin { get; set; }

        [BindProperty]
        public string TipoMonto { get; set; } = "total";

        public string? MensajeError { get; private set; }

        public bool MostrarResultados { get; private set; }

        public decimal MontoTotal { get; private set; }

        public int CantidadDTE { get; private set; }

        // Se conserva el JSON para recorrerlo en Razor.
        public string RespuestaJson { get; private set; } = "";

        public ResumenRangoModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!FechaInicio.HasValue || !FechaFin.HasValue)
            {
                MensajeError =
                    "Debe seleccionar ambas fechas.";

                return Page();
            }

            if (FechaInicio.Value.Date >
                FechaFin.Value.Date)
            {
                MensajeError =
                    "La fecha inicial no puede superar la fecha final.";

                return Page();
            }

            if (TipoMonto != "total" && TipoMonto != "siniva")
            {
                MensajeError =
                    "Seleccione un tipo de monto valido.";

                return Page();
            }

            string inicio = FechaInicio.Value.ToString(
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
            );

            string fin = FechaFin.Value.ToString(
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
            );

            bool incluirIva = TipoMonto == "total";

            try
            {
                string respuesta = await clienteApi.ConsultarRango(
                    inicio,
                    fin,
                    incluirIva
                );

                using JsonDocument documento =
                    JsonDocument.Parse(respuesta);

                JsonElement raiz = documento.RootElement;

                MontoTotal = raiz
                    .GetProperty("montoTotal")
                    .GetDecimal();

                JsonElement detalle =
                    raiz.GetProperty("detalle");

                if (detalle.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        "El detalle no es un arreglo JSON."
                    );
                }

                // Recorrer los registros sin utilizar
                // una lista nativa para almacenarlos.
                foreach (JsonElement registro in
                    detalle.EnumerateArray())
                {
                    CantidadDTE += registro
                        .GetProperty("cantidadDTE")
                        .GetInt32();
                }

                RespuestaJson = respuesta;
                MostrarResultados = true;
            }
            catch (HttpRequestException ex)
            {
                MensajeError =
                    "No se pudo consultar el Backend: " +
                    ex.Message;
            }
            catch (TaskCanceledException)
            {
                MensajeError =
                    "La solicitud excedio el tiempo de espera.";
            }
            catch (JsonException)
            {
                MensajeError =
                    "La respuesta del Backend no tiene el formato esperado.";
            }
            catch (InvalidOperationException)
            {
                MensajeError =
                    "Los datos de la API tienen un tipo incorrecto.";
            }
            catch (KeyNotFoundException)
            {
                MensajeError =
                    "Faltan campos necesarios en la respuesta.";
            }

            return Page();
        }
    }
}
