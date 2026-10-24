
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoResumenIva
    {
        public ResumenIva Dato { get; set; }

        public NodoResumenIva? Siguiente { get; set; }

        public NodoResumenIva(ResumenIva dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
