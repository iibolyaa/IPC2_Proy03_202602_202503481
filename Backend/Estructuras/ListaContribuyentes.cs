
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaContribuyentes
    {
        private NodoContribuyente? primero;
        private NodoContribuyente? ultimo;
        private int cantidad;

        public ListaContribuyentes()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        // Obtener la cantidad de contribuyentes
        public int ObtenerCantidad()
        {
            return cantidad;
        }

        // Comprobar si la lista esta vacia
        public bool EstaVacia()
        {
            return primero == null;
        }

        // Buscar un contribuyente por NIT
        public Contribuyente? Buscar(string nit)
        {
            NodoContribuyente? actual = primero;

            while (actual != null)
            {
                if (actual.Dato.Nit == nit)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        // Insertar un contribuyente nuevo
        public bool Insertar(Contribuyente contribuyente)
        {
            // No permitir NIT repetidos.
            if (Buscar(contribuyente.Nit) != null)
            {
                return false;
            }

            NodoContribuyente nuevo = new NodoContribuyente(contribuyente);

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
            return true;
        }

        // Actualizar el nombre de un contribuyente
        public bool Actualizar(string nit, string nombre)
        {
            Contribuyente? encontrado = Buscar(nit);

            if (encontrado == null)
            {
                return false;
            }

            encontrado.Nombre = nombre;
            return true;
        }

        // Obtener un contribuyente por posicion
        public Contribuyente? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoContribuyente? actual = primero;
            int contador = 0;

            while (actual != null)
            {
                if (contador == indice)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
                contador++;
            }

            return null;
        }

        // Eliminar todos los nodos
        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
