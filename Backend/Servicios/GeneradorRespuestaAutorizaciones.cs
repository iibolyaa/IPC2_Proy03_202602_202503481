using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Backend.Modelos;

namespace Backend.Ayuda
{
    public class GeneradorRespuestaAutorizaciones
    {
        public string Generar(ResultadoAutorizaciones resultado)
        {
            using MemoryStream memoria = new MemoryStream();

            using (Utf8JsonWriter escritor = new Utf8JsonWriter(memoria, new JsonWriterOptions
                    {
                        Indented = true
                    }))
            {
                escritor.WriteStartObject();

                escritor.WritePropertyName("resultadoAutorizaciones");

                escritor.WriteStartObject();

                escritor.WriteNumber(
                    "cantidadAutorizacionesAprobadas",
                    resultado.CantidadAutorizacionesAprobadas
                );

                escritor.WriteNumber(
                    "cantidadAutorizacionesRechazadasPorNitEmisorInexistente",
                    resultado.CantidadAutorizacionesRechazadasPorNitEmisorInexistente
                );

                escritor.WriteNumber(
                    "cantidadAutorizacionesRechazadasPorNitReceptorInexistente",
                    resultado.CantidadAutorizacionesRechazadasPorNitReceptorInexistente
                );

                escritor.WritePropertyName("detalleAprobaciones");

                escritor.WriteStartArray();

                for (int i = 0; i < resultado.DetalleAprobaciones.ObtenerCantidad(); i++)
                {
                    DetalleAprobacion? detalle = resultado.DetalleAprobaciones.ObtenerPorIndice(i);

                    if (detalle == null)
                    {
                        continue;
                    }

                    escritor.WriteStartObject();

                    escritor.WriteString("referencia", detalle.Referencia);

                    escritor.WriteNumber("codigoAprobacion", detalle.CodigoAprobacion);

                    escritor.WriteEndObject();
                }

                escritor.WriteEndArray();

                escritor.WriteEndObject();

                escritor.WriteEndObject();

                escritor.Flush();
            }

            return Encoding.UTF8.GetString(
                memoria.ToArray()
            );
        }
    }
}
