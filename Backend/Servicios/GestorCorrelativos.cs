
using System;
using System.Globalization;
using Backend.Estructuras;

namespace Backend.Ayuda
{
    public class GestorCorrelativos
    {
        private readonly ListaCorrelativos correlativos;
        private readonly object bloqueo;

        public GestorCorrelativos(ListaCorrelativos correlativos)
        {
            this.correlativos = correlativos;
            bloqueo = new object();
        }

        // Generar un codigo de autorizacion.
        public string GenerarCodigo(DateTime fechaAutorizacion)
        {
            lock (bloqueo)
            {
                // Obtener el siguiente numero disponible.
                int numero = correlativos.SiguienteCorrelativo(fechaAutorizacion.Date);

                // Formato de fecha: yyyymmdd.
                string fechaTexto =
                    fechaAutorizacion.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

                // Correlativo de ocho posiciones.
                string numeroTexto = numero.ToString("D8", CultureInfo.InvariantCulture);

                // Concatenar fecha y correlativo.
                return fechaTexto + numeroTexto;
            }
        }

        // Consultar el ultimo numero utilizado.
        public int ConsultarUltimoNumero(DateTime fecha)
        {
            lock (bloqueo)
            {
                var encontrado = correlativos.BuscarPorFecha(fecha.Date);

                if (encontrado == null)
                {
                    return 0;
                }

                return encontrado.UltimoNumero;
            }
        }

        // Limpiar los correlativos en memoria.
        public void Limpiar()
        {
            lock (bloqueo)
            {
                correlativos.Limpiar();
            }
        }
    }
}
