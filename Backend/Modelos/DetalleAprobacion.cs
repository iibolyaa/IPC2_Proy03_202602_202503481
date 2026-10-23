using System.Text.Json.Serialization;

namespace Backend.Modelos
{
    public class DetalleAprobacion
    {
        [JsonPropertyName("referencia")]
        public string Referencia { get; set; }

        [JsonPropertyName("codigoAprobacion")]
        public long CodigoAprobacion { get; set; }

        public DetalleAprobacion(string referencia, long codigoAprobacion)
        {
            Referencia = referencia;
            CodigoAprobacion = codigoAprobacion;
        }
    }
}