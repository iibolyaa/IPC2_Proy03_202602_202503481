
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaErrores
    {
        private NodoError? primero;
        private NodoError? ultimo;
        private int cantidad;

        public ListaErrores()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        // Obtener la cantidad de errores registrados
        public int ObtenerCantidad()
        {
            return cantidad;
        }

        // Comprobar si la lista esta vacia
        public bool EstaVacia()
        {
            return primero == null;
        }

        // Agregar un error al final
        public void Insertar(ErrorFactura error)
        {
            NodoError nuevo = new NodoError(error);

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

        // Obtener un error por posicion.
        public ErrorFactura? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoError? actual = primero;
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

        // Contar errores de un tipo especifico
        // dentro de un rango de fechas
        public int ContarPorTipo(string tipoError, DateTime fechaInicio, DateTime fechaFin)
        {
            int contador = 0;

            NodoError? actual = primero;

            while (actual != null)
            {
                ErrorFactura error = actual.Dato;

                if (error.TipoError == tipoError && error.Fecha.Date >= fechaInicio.Date &&
                    error.Fecha.Date <= fechaFin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Contar todos los errores de un rango
        public int ContarPorRango(DateTime fechaInicio, DateTime fechaFin)
        {
            int contador = 0;

            NodoError? actual = primero;

            while (actual != null)
            {
                ErrorFactura error = actual.Dato;

                if (error.Fecha.Date >= fechaInicio.Date && error.Fecha.Date <= fechaFin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Eliminar todos los errores almacenados.
        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
