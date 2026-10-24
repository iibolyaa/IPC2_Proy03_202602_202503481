
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class NodoResumenFecha
    {
        public ResumenFecha Dato { get; set; }

        public NodoResumenFecha? Siguiente { get; set; }

        public NodoResumenFecha(ResumenFecha dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
