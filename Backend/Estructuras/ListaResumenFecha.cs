
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaResumenFecha
    {
        private NodoResumenFecha? primero;
        private NodoResumenFecha? ultimo;
        private int cantidad;

        public ListaResumenFecha()
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

        // Buscar un resumen correspondiente a una fecha.
        public ResumenFecha? BuscarPorFecha(DateTime fecha)
        {
            NodoResumenFecha? actual = primero;

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

        // Acumular valores de un DTE autorizado.
        public void AgregarMovimiento(DateTime fecha, decimal valor, decimal total)
        {
            ResumenFecha? resumen = BuscarPorFecha(fecha);

            if (resumen != null)
            {
                resumen.AgregarDTE(valor, total);
                return;
            }

            ResumenFecha nuevoResumen = new ResumenFecha(fecha);

            nuevoResumen.AgregarDTE(valor, total);

            NodoResumenFecha nuevo = new NodoResumenFecha(nuevoResumen);

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

        // Obtener resumen por posicion.
        public ResumenFecha? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoResumenFecha? actual = primero;
            int posicion = 0;

            while (actual != null)
            {
                if (posicion == indice)
                {
                    return actual.Dato;
                }

                posicion++;
                actual = actual.Siguiente;
            }

            return null;
        }

        // Obtener total autorizado con IVA.
        public decimal ObtenerTotalConIva()
        {
            decimal total = 0;

            NodoResumenFecha? actual = primero;

            while (actual != null)
            {
                total += actual.Dato.TotalConIva;
                actual = actual.Siguiente;
            }

            return total;
        }

        // Obtener total autorizado sin IVA.
        public decimal ObtenerTotalSinIva()
        {
            decimal total = 0;

            NodoResumenFecha? actual = primero;

            while (actual != null)
            {
                total += actual.Dato.TotalSinIva;
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
