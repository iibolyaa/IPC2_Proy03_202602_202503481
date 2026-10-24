
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Backend.Ayuda;
using Backend.Estructuras;
using Backend.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/consultas")]
    public class ConsultasController : ControllerBase
    {
        private readonly GestorConsultas gestor;
        private readonly ValidadorRango validador;

        public ConsultasController(EstadoSistema estado)
        {
            gestor = new GestorConsultas(estado);
            validador = new ValidadorRango();
        }

        // GET: api/consultas/contribuyente
        [HttpGet("contribuyente")]
        public IActionResult ConsultarContribuyente([FromQuery] string nit, [FromQuery] string inicio,
            [FromQuery] string fin, [FromQuery] string tipo)
        {
            if (!validador.Validar(inicio, fin, out DateTime fechaInicio, out DateTime fechaFin))
            {
                return BadRequest("El rango de fechas no es valido. Use dd/MM/yyyy.");
            }

            if (string.IsNullOrWhiteSpace(nit))
            {
                return BadRequest("Debe indicar un NIT.");
            }

            string tipoNormalizado = tipo.ToLowerInvariant();

            if (tipoNormalizado != "emitidas" && tipoNormalizado != "recibidas")
            {
                return BadRequest("El tipo debe ser emitidas o recibidas.");
            }

            bool emitidas = tipoNormalizado == "emitidas";

            ListaResumenContribuyente resumen = gestor.ConsultarContribuyente(nit, fechaInicio,
                    fechaFin, emitidas);

            using MemoryStream memoria = new MemoryStream();

            using (Utf8JsonWriter escritor = new Utf8JsonWriter(memoria))
            {
                escritor.WriteStartObject();

                escritor.WriteString("nit", nit);
                escritor.WriteString("tipo", tipoNormalizado);
                escritor.WriteString("inicio", inicio);
                escritor.WriteString("fin", fin);

                escritor.WriteNumber("cantidadTotal", resumen.ObtenerTotalDTE());

                escritor.WriteNumber("montoTotal", resumen.ObtenerMontoTotal());

                escritor.WritePropertyName("detalle");
                escritor.WriteStartArray();

                for (int i = 0; i < resumen.ObtenerCantidad(); i++)
                {
                    ResumenContribuyente? dato = resumen.ObtenerPorIndice(i);

                    if (dato == null)
                    {
                        continue;
                    }

                    escritor.WriteStartObject();

                    escritor.WriteString("fecha", dato.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

                    escritor.WriteNumber("cantidadDTE", dato.CantidadDTE);

                    escritor.WriteNumber("montoTotal", dato.MontoTotal);

                    escritor.WriteEndObject();
                }

                escritor.WriteEndArray();
                escritor.WriteEndObject();
                escritor.Flush();
            }

            return Content(Encoding.UTF8.GetString(memoria.ToArray()), "application/json");
        }

        // GET: api/consultas/iva
        [HttpGet("iva")]
        public IActionResult ConsultarIva([FromQuery] string fecha)
        {
            if (!validador.ValidarFecha(fecha, out DateTime fechaConsulta))
            {
                return BadRequest("La fecha no es valida. Use dd/MM/yyyy.");
            }

            ListaResumenIva resumen = gestor.ConsultarIva(fechaConsulta);

            using MemoryStream memoria = new MemoryStream();

            using (Utf8JsonWriter escritor = new Utf8JsonWriter(memoria))
            {
                escritor.WriteStartObject();

                escritor.WriteString("fecha", fecha);

                escritor.WritePropertyName("detalle");
                escritor.WriteStartArray();

                for (int i = 0; i < resumen.ObtenerCantidad(); i++)
                {
                    ResumenIva? dato = resumen.ObtenerPorIndice(i);

                    if (dato == null)
                    {
                        continue;
                    }

                    escritor.WriteStartObject();

                    escritor.WriteString("nit", dato.Nit);

                    escritor.WriteNumber("ivaEmitido", dato.IvaEmitido);

                    escritor.WriteNumber("ivaRecibido", dato.IvaRecibido);

                    escritor.WriteEndObject();
                }

                escritor.WriteEndArray();
                escritor.WriteEndObject();
                escritor.Flush();
            }

            return Content(Encoding.UTF8.GetString(memoria.ToArray()), "application/json");
        }

        // GET: api/consultas/rango
        [HttpGet("rango")]
        public IActionResult ConsultarRango([FromQuery] string inicio, [FromQuery] string fin,
            [FromQuery] bool incluirIva = true)
        {
            if (!validador.Validar(inicio, fin,
                out DateTime fechaInicio,
                out DateTime fechaFin))
            {
                return BadRequest("El rango de fechas no es valido. Use dd/MM/yyyy.");
            }

            ListaResumenFecha resumen = gestor.ConsultarRango(fechaInicio, fechaFin);

            using MemoryStream memoria = new MemoryStream();

            using (Utf8JsonWriter escritor = new Utf8JsonWriter(memoria))
            {
                escritor.WriteStartObject();

                escritor.WriteString("inicio", inicio);
                escritor.WriteString("fin", fin);

                escritor.WriteBoolean("incluirIva", incluirIva);

                decimal montoTotal = incluirIva
                    ? resumen.ObtenerTotalConIva()
                    : resumen.ObtenerTotalSinIva();

                escritor.WriteNumber("montoTotal", montoTotal);

                escritor.WritePropertyName("detalle");
                escritor.WriteStartArray();

                for (int i = 0; i < resumen.ObtenerCantidad(); i++)
                {
                    ResumenFecha? dato = resumen.ObtenerPorIndice(i);

                    if (dato == null)
                    {
                        continue;
                    }

                    escritor.WriteStartObject();

                    escritor.WriteString("fecha", dato.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

                    escritor.WriteNumber("cantidadDTE", dato.CantidadDTE);

                    escritor.WriteNumber("monto", dato.ObtenerMonto(incluirIva));

                    escritor.WriteEndObject();
                }

                escritor.WriteEndArray();
                escritor.WriteEndObject();
                escritor.Flush();
            }

            return Content(Encoding.UTF8.GetString(memoria.ToArray()), "application/json");
        }
    }
}
