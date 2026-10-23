using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoAprobacion
    {
        public DetalleAprobacion Dato { get; set; }

        public NodoAprobacion? Siguiente { get; set; }

        public NodoAprobacion(DetalleAprobacion dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
