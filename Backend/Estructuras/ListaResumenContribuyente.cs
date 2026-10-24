
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaResumenContribuyente
    {
        private NodoResumenContribuyente? primero;
        private NodoResumenContribuyente? ultimo;
        private int cantidad;

        public ListaResumenContribuyente()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        public int ObtenerCantidad()
        {
            return cantidad;
        }

        public bool EstaVacia()
        {
            return primero == null;
        }

        // Buscar un resumen correspondiente a una fecha
        public ResumenContribuyente? BuscarPorFecha(DateTime fecha)
        {
            NodoResumenContribuyente? actual = primero;

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

        // Agregar un movimiento a la fecha correspondiente.
        public void AgregarMovimiento(DateTime fecha, decimal monto)
        {
            ResumenContribuyente? resumen = BuscarPorFecha(fecha);

            if (resumen != null)
            {
                resumen.AgregarDTE(monto);
                return;
            }

            ResumenContribuyente nuevoResumen = new ResumenContribuyente(fecha);

            nuevoResumen.AgregarDTE(monto);

            NodoResumenContribuyente nuevo = new NodoResumenContribuyente(nuevoResumen);

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

        // Obtener un resumen por posicion.
        public ResumenContribuyente? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoResumenContribuyente? actual = primero;
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

        // Calcular el monto total de todas las fechas.
        public decimal ObtenerMontoTotal()
        {
            decimal total = 0;

            NodoResumenContribuyente? actual = primero;

            while (actual != null)
            {
                total += actual.Dato.MontoTotal;
                actual = actual.Siguiente;
            }

            return total;
        }

        // Calcular la cantidad total de documentos.
        public int ObtenerTotalDTE()
        {
            int total = 0;

            NodoResumenContribuyente? actual = primero;

            while (actual != null)
            {
                total += actual.Dato.CantidadDTE;
                actual = actual.Siguiente;
            }

            return total;
        }

        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
