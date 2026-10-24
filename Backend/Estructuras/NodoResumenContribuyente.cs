
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoResumenContribuyente
    {
        public ResumenContribuyente Dato { get; set; }

        public NodoResumenContribuyente? Siguiente { get; set; }

        public NodoResumenContribuyente(ResumenContribuyente dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
