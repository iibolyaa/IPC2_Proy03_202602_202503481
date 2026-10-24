
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Servicios
{
    public class ClienteApi
    {
        private readonly HttpClient cliente;

        public ClienteApi(HttpClient cliente)
        {
            this.cliente = cliente;
        }

        // Enviar JSON al Backend.
        private async Task<string> EnviarPost(string ruta, string contenidoJson)
        {
            using StringContent contenido = new StringContent(contenidoJson, Encoding.UTF8, "application/json");

            using HttpResponseMessage respuesta =await cliente.PostAsync(ruta,contenido);

            string resultado =await respuesta.Content.ReadAsStringAsync();

            if (!respuesta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Error HTTP {(int)respuesta.StatusCode}: " +
                    resultado
                );
            }

            return resultado;
        }

        // Realizar consultas GET.
        private async Task<string> EnviarGet(string ruta)
        {
            using HttpResponseMessage respuesta = await cliente.GetAsync(ruta);

            string resultado = await respuesta.Content.ReadAsStringAsync();

            if (!respuesta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Error HTTP {(int)respuesta.StatusCode}: " +
                    resultado
                );
            }

            return resultado;
        }

        // Actualizar los contribuyentes del RTU.
        public async Task<string> ActualizarRtu(string contenidoJson)
        {
            return await EnviarPost( "api/rtu/actualizar", contenidoJson);
        }

        // Procesar solicitudes de autorizacion DTE.
        public async Task<string> ProcesarDTE(string contenidoJson)
        {
            return await EnviarPost("api/autorizaciones/procesar", contenidoJson);
        }

        // Consultar un contribuyente.
        public async Task<string> ConsultarContribuyente(string nit, string inicio, string fin, string tipo)
        {
            string ruta = "api/consultas/contribuyente" +"?nit=" + Uri.EscapeDataString(nit) +                "&inicio=" + Uri.EscapeDataString(inicio) +
                "&fin=" + Uri.EscapeDataString(fin) +
                "&tipo=" + Uri.EscapeDataString(tipo);

            return await EnviarGet(ruta);
        }

        // Consultar resumen de IVA.
        public async Task<string> ConsultarIva(string fecha)
        {
            string ruta = "api/consultas/iva" + "?fecha=" + Uri.EscapeDataString(fecha);

            return await EnviarGet(ruta);
        }

        // Consultar resumen por rango de fechas.
        public async Task<string> ConsultarRango(string inicio, string fin, bool incluirIva)
        {
            string ruta = "api/consultas/rango" + "?inicio=" + Uri.EscapeDataString(inicio) +
                "&fin=" + Uri.EscapeDataString(fin) + "&incluirIva=" +
                incluirIva.ToString().ToLowerInvariant();

            return await EnviarGet(ruta);
        }

        // Consultar un contribuyente del RTU.
        public async Task<string> BuscarContribuyente(string nit)
        {
            string ruta = "api/rtu/consultar/" + Uri.EscapeDataString(nit);

            return await EnviarGet(ruta);
        }

        // Obtener la cantidad de contribuyentes.
        public async Task<string> CantidadContribuyentes()
        {
            return await EnviarGet("api/rtu/cantidad");
        }
    }
}
