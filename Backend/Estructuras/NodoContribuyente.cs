
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoContribuyente
    {
        // Contribuyente almacenado en el nodo.
        public Contribuyente Dato { get; set; }

        // Referencia al siguiente nodo.
        public NodoContribuyente? Siguiente { get; set; }

        // Constructor.
        public NodoContribuyente(Contribuyente dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
