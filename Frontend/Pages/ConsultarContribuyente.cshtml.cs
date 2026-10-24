using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Frontend.Pages
{
    public class ConsultarContribuyenteModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty]
        public string Nit { get; set; } = "";

        [BindProperty]
        public string Tipo { get; set; } = "emitidas";

        [BindProperty]
        public DateTime? FechaInicio { get; set; }

        [BindProperty]
        public DateTime? FechaFin { get; set; }

        public string? MensajeError { get; private set; }

        public bool MostrarResultados { get; private set; }

        public int CantidadTotal { get; private set; }

        public decimal MontoTotal { get; private set; }

        // Conservamos el resultado JSON para recorrer
        // su arreglo sin utilizar List<T>.
        public string RespuestaJson { get; private set; } = "";

        public ConsultarContribuyenteModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Nit))
            {
                MensajeError = "Debe ingresar el NIT del contribuyente.";

                return Page();
            }

            Nit = Nit.Trim();

            if (Tipo != "emitidas" && Tipo != "recibidas")
            {
                MensajeError = "Seleccione documentos emitidos o recibidos.";

                return Page();
            }

            if (!FechaInicio.HasValue || !FechaFin.HasValue)
            {
                MensajeError = "Debe seleccionar ambas fechas.";

                return Page();
            }

            if (FechaInicio.Value.Date > FechaFin.Value.Date)
            {
                MensajeError = "La fecha inicial no puede ser posterior a la final.";

                return Page();
            }

            string inicio = FechaInicio.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            string fin = FechaFin.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            try
            {
                string respuesta = await clienteApi.ConsultarContribuyente(Nit, inicio, fin, Tipo);

                using JsonDocument documento = JsonDocument.Parse(respuesta);

                JsonElement raiz = documento.RootElement;

                CantidadTotal = raiz.GetProperty("cantidadTotal").GetInt32();

                MontoTotal = raiz
                    .GetProperty("montoTotal")
                    .GetDecimal();

                JsonElement detalle =
                    raiz.GetProperty("detalle");

                if (detalle.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        "El detalle recibido no es un arreglo."
                    );
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
                    "Los datos recibidos no tienen el tipo esperado.";
            }
            catch (KeyNotFoundException)
            {
                MensajeError =
                    "Faltan campos necesarios en la respuesta de la API.";
            }

            return Page();
        }
    }
}
