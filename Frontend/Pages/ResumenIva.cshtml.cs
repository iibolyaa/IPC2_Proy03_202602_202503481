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
    public class ResumenIvaModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty]
        public DateTime? Fecha { get; set; }

        public string? MensajeError { get; private set; }

        public bool MostrarResultados { get; private set; }

        public decimal TotalIvaEmitido { get; private set; }

        public decimal TotalIvaRecibido { get; private set; }

        public int CantidadContribuyentes { get; private set; }

        // Respuesta original del Backend
        public string RespuestaJson { get; private set; } = "";

        public ResumenIvaModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Fecha.HasValue)
            {
                MensajeError =
                    "Debe seleccionar una fecha.";

                return Page();
            }

            string fechaTexto = Fecha.Value.ToString(
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture
            );

            try
            {
                string respuesta =
                    await clienteApi.ConsultarIva(fechaTexto);

                using JsonDocument documento =
                    JsonDocument.Parse(respuesta);

                JsonElement raiz = documento.RootElement;

                JsonElement detalle =
                    raiz.GetProperty("detalle");

                if (detalle.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException(
                        "El detalle recibido no es un arreglo."
                    );
                }

                // Recorrer directamente los elementos JSON.
                foreach (JsonElement registro in
                    detalle.EnumerateArray())
                {
                    decimal emitido = registro
                        .GetProperty("ivaEmitido")
                        .GetDecimal();

                    decimal recibido = registro
                        .GetProperty("ivaRecibido")
                        .GetDecimal();

                    TotalIvaEmitido += emitido;
                    TotalIvaRecibido += recibido;
                    CantidadContribuyentes++;
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
                    "La consulta excedio el tiempo de espera.";
            }
            catch (JsonException)
            {
                MensajeError =
                    "La respuesta de la API no tiene el formato esperado.";
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
