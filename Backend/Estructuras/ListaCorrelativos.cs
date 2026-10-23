
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaCorrelativos
    {
        private NodoCorrelativo? primero;
        private NodoCorrelativo? ultimo;
        private int cantidad;

        public ListaCorrelativos()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        // Obtener cantidad de fechas registradas
        public int ObtenerCantidad()
        {
            return cantidad;
        }

        // Comprobar si la lista esta vacia
        public bool EstaVacia()
        {
            return primero == null;
        }

        // Buscar el correlativo correspondiente a una fecha
        public Correlativo? BuscarPorFecha(DateTime fecha)
        {
            NodoCorrelativo? actual = primero;

            while (actual != null)
            {
                if (actual.Dato.Fecha.Date == fecha.Date)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        // Insertar una nueva fecha y su correlativo
        public bool Insertar(Correlativo correlativo)
        {
            if (BuscarPorFecha(correlativo.Fecha) != null)
            {
                return false;
            }

            NodoCorrelativo nuevo = new NodoCorrelativo(correlativo);

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

        // Generar el siguiente correlativo de una fecha
        public int SiguienteCorrelativo(DateTime fecha)
        {
            Correlativo? encontrado = BuscarPorFecha(fecha);

            if (encontrado != null)
            {
                if (encontrado.UltimoNumero >= 99999999)
                {
                    throw new InvalidOperationException("Se alcanzo el limite diario de correlativos.");
                }

                encontrado.UltimoNumero++;
                return encontrado.UltimoNumero;
            }

            Correlativo nuevo = new Correlativo(fecha, 1);
            Insertar(nuevo);

            return 1;
        }

        // Obtener un correlativo por posicion
        public Correlativo? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoCorrelativo? actual = primero;
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

        // Limpiar todos los correlativos
        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
