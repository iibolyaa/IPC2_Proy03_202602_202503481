
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaAprobaciones
    {
        private NodoAprobacion? primero;
        private NodoAprobacion? ultimo;
        private int cantidad;

        public ListaAprobaciones()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        public void Insertar(DetalleAprobacion detalle)
        {
            NodoAprobacion nuevo = new NodoAprobacion(detalle);

            if (primero == null)
            {
                primero = nuevo;
                ultimo = nuevo;
            }
            else
            {
                ultimo!.Siguiente = nuevo;
                ultimo = nuevo;
            }

            cantidad++;
        }

        public int ObtenerCantidad()
        {
            return cantidad;
        }

        public DetalleAprobacion? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoAprobacion? actual = primero;
            int posicion = 0;

            while (actual != null)
            {
                if (posicion == indice)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
                posicion++;
            }

            return null;
        }
    }
}
