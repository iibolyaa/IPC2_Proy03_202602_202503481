using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class EnviarMensajeModel : PageModel
    {
        private readonly ClienteApi clienteApi;

        [BindProperty]
        public string TipoMensaje { get; set; } = "rtu";

        [BindProperty]
        public IFormFile? ArchivoJson { get; set; }

        public string? MensajeError { get; private set; }
        public string? MensajeExito { get; private set; }
        public string? RespuestaJson { get; private set; }

        public EnviarMensajeModel(ClienteApi clienteApi)
        {
            this.clienteApi = clienteApi;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TipoMensaje != "rtu" &&
                TipoMensaje != "dte")
            {
                MensajeError = "Seleccione un tipo de mensaje valido.";
                return Page();
            }

            if (ArchivoJson == null || ArchivoJson.Length == 0)
            {
                MensajeError = "Debe seleccionar un archivo JSON.";
                return Page();
            }

            if (!string.Equals(
                Path.GetExtension(ArchivoJson.FileName),
                ".json",
                StringComparison.OrdinalIgnoreCase))
            {
                MensajeError = "Solamente se permiten archivos .json.";
                return Page();
            }

            // Limite preventivo de 10 MB.
            const long tamanoMaximo = 10 * 1024 * 1024;

            if (ArchivoJson.Length > tamanoMaximo)
            {
                MensajeError = "El archivo supera el limite de 10 MB.";
                return Page();
            }

            try
            {
                string contenido;

                using (Stream flujo = ArchivoJson.OpenReadStream())
                using (StreamReader lector = new StreamReader(
                    flujo,
                    new UTF8Encoding(false, true),
                    detectEncodingFromByteOrderMarks: true))
                {
                    contenido = await lector.ReadToEndAsync();
                }

                // Comprobar que el contenido sea JSON.
                using (JsonDocument documento =
                    JsonDocument.Parse(contenido))
                {
                    if (documento.RootElement.ValueKind !=
                        JsonValueKind.Array)
                    {
                        MensajeError =
                            "El archivo debe contener un arreglo JSON.";
                        return Page();
                    }
                }

                // Enviar el contenido original a la API.
                if (TipoMensaje == "rtu")
                {
                    RespuestaJson =
                        await clienteApi.ActualizarRtu(contenido);
                }
                else
                {
                    RespuestaJson =
                        await clienteApi.ProcesarDTE(contenido);
                }

                MensajeExito =
                    "El Backend recibio y respondio la solicitud.";
            }
            catch (JsonException)
            {
                MensajeError =
                    "El archivo no contiene un JSON valido.";
            }
            catch (DecoderFallbackException)
            {
                MensajeError =
                    "El archivo contiene texto con codificacion invalida.";
            }
            catch (HttpRequestException ex)
            {
                MensajeError =
                    "Error de comunicacion con la API: " + ex.Message;
            }
            catch (TaskCanceledException)
            {
                MensajeError =
                    "La solicitud tardo demasiado en responder.";
            }
            catch (IOException ex)
            {
                MensajeError =
                    "No se pudo leer el archivo: " + ex.Message;
            }

            return Page();
        }
    }
}
