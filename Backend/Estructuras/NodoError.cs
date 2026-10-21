
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoError
    {
        // Error almacenado en el nodo.
        public ErrorFactura Dato { get; set; }

        // Referencia al siguiente nodo.
        public NodoError? Siguiente { get; set; }

        public NodoError(ErrorFactura dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
