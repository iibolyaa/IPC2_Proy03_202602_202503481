
using System.Text.Json.Serialization;

namespace Backend.Modelos
{
    public class RespuestaRtu
    {
        [JsonPropertyName("CONTRIBUYENTES_NUEVOS")]
        public int ContribuyentesNuevos { get; set; }

        [JsonPropertyName("CONTRIBUYENTES_ACTUALIZADOS")]
        public int ContribuyentesActualizados { get; set; }

        [JsonPropertyName("NITS_INVALIDOS")]
        public int NitsInvalidos { get; set; }

        public RespuestaRtu()
        {
            ContribuyentesNuevos = 0;
            ContribuyentesActualizados = 0;
            NitsInvalidos = 0;
        }
    }
}
