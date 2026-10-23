
using System;
using System.Text.Json;
using Backend.Ayuda;
using Backend.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/autorizaciones")]
    public class AutorizacionesController : ControllerBase
    {
        private readonly GestorAutorizaciones gestor;
        private readonly GeneradorRespuestaAutorizaciones generador;

        public AutorizacionesController(EstadoSistema estado)
        {
            gestor = new GestorAutorizaciones(estado);

            generador = new GeneradorRespuestaAutorizaciones();
        }

        // POST: api/autorizaciones/procesar
        [HttpPost("procesar")]
        public IActionResult Procesar(
            [FromBody] JsonElement contenido)
        {
            // La entrada debe ser un arreglo JSON.
            if (contenido.ValueKind != JsonValueKind.Array)
            {
                return BadRequest("Se esperaba un arreglo JSON de DTE.");
            }

            // Revisar la estructura antes de procesar.
            foreach (JsonElement elemento in contenido.EnumerateArray())
            {
                if (elemento.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest("Cada solicitud debe ser un objeto JSON.");
                }

                try
                {
                    SolicitudDTE? solicitud = elemento.Deserialize<SolicitudDTE>();

                    if (solicitud == null)
                    {
                        return BadRequest("Se encontro una solicitud nula.");
                    }
                }
                catch (JsonException)
                {
                    return BadRequest("Uno de los DTE contiene tipos de datos incorrectos.");
                }
            }

            // Preparar el resultado del archivo.
            ResultadoAutorizaciones resultado = new ResultadoAutorizaciones();

            // Obtener la fecha local de Guatemala.
            TimeZoneInfo zonaGuatemala = TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");

            DateTime fechaAutorizacion = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaGuatemala);

            // Procesar cada solicitud del arreglo.
            foreach (JsonElement elemento in contenido.EnumerateArray())
            {
                SolicitudDTE solicitud = elemento.Deserialize<SolicitudDTE>()!;

                gestor.ProcesarSolicitud(solicitud, resultado, fechaAutorizacion);
            }

            // Generar el JSON de respuesta.
            string respuesta = generador.Generar(resultado);

            return Content(respuesta, "application/json");
        }
    }
}
