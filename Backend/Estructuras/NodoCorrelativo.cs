
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoCorrelativo
    {
        // Correlativo almacenado en el nodo
        public Correlativo Dato { get; set; }
        public NodoCorrelativo? Siguiente { get; set; }
        public NodoCorrelativo(Correlativo dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
