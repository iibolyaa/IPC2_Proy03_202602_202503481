
using System;
using System.Text.Json;
using Backend.Ayuda;
using Backend.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/rtu")]
    public class RtuController : ControllerBase
    {
        private readonly EstadoSistema estado;
        private readonly GestorRtu gestor;

        public RtuController(EstadoSistema estado)
        {
            this.estado = estado;

            gestor = new GestorRtu(estado.Contribuyentes);
        }

        // POST: api/rtu/actualizar
        [HttpPost("actualizar")]
        public IActionResult Actualizar(
            [FromBody] JsonElement contenido)
        {
            // Verificar que la entrada sea un arreglo.
            if (contenido.ValueKind != JsonValueKind.Array)
            {
                return BadRequest("Se esperaba un arreglo JSON de contribuyentes.");
            }

            // Verificar la estructura de todos los elementos.
            foreach (JsonElement elemento in contenido.EnumerateArray())
            {
                if (elemento.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest("Cada contribuyente debe ser un objeto JSON.");
                }

                try
                {
                    SolicitudContribuyente? solicitud = elemento.Deserialize<SolicitudContribuyente>();

                    if (solicitud == null)
                    {
                        return BadRequest("Se encontro un contribuyente nulo.");
                    }
                }
                catch (JsonException)
                {
                    return BadRequest("Los datos de un contribuyente tienen un formato incorrecto.");
                }
            }

            RespuestaRtu respuesta = new RespuestaRtu();

            // Procesar los contribuyentes bajo
            // un unico bloqueo y guardado.
            estado.EjecutarCambio(() =>
            {
                foreach (JsonElement elemento in contenido.EnumerateArray())
                {
                    SolicitudContribuyente solicitud = elemento.Deserialize<SolicitudContribuyente>()!;

                    gestor.ProcesarContribuyente(solicitud.Nit, solicitud.Nombre, respuesta);
                }
            });

            return Ok(respuesta);
        }

        // GET: api/rtu/consultar/7378106
        [HttpGet("consultar/{nit}")]
        public IActionResult Consultar(string nit)
        {
            Contribuyente? contribuyente = estado.Consultar(() => gestor.Buscar(nit));

            if (contribuyente == null)
            {
                return NotFound("El contribuyente no existe en el RTU.");
            }

            return Ok(contribuyente);
        }

        // GET: api/rtu/cantidad
        [HttpGet("cantidad")]
        public IActionResult Cantidad()
        {
            int cantidad = estado.Consultar(() => gestor.ObtenerCantidad());

            return Ok(new
            {
                cantidadContribuyentes = cantidad
            });
        }
    }
}
