
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoSolicitud
    {
        public SolicitudProcesada Dato { get; set; }

        public NodoSolicitud? Siguiente { get; set; }

        public NodoSolicitud(SolicitudProcesada dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}