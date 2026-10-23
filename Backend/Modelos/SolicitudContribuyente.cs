// Leer solicitudes de registro de contribuyentes. Interpreta los datos de entrada.


using System.Text.Json.Serialization;

namespace Backend.Modelos
{
    public class SolicitudContribuyente
    {
        [JsonPropertyName("NIT")]
        public string? Nit { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }
    }
}
