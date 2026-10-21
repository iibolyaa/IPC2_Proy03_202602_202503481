
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoFactura
    {
        // DTE almacenado en el nodo.
        public Factura Dato { get; set; }

        // Referencia al siguiente nodo.
        public NodoFactura? Siguiente { get; set; }

        // Constructor.
        public NodoFactura(Factura dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
