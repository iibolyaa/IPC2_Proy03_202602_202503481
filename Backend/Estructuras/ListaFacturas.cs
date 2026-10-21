
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaFacturas
    {
        private NodoFactura? primero;
        private NodoFactura? ultimo;
        private int cantidad;

        public ListaFacturas()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        // Obtener la cantidad total de DTE.
        public int ObtenerCantidad()
        {
            return cantidad;
        }

        // Comprobar si la lista esta vacia.
        public bool EstaVacia()
        {
            return primero == null;
        }

        // Buscar un DTE por su referencia.
        public Factura? BuscarPorReferencia(string referencia)
        {
            NodoFactura? actual = primero;

            while (actual != null)
            {
                if (actual.Dato.Referencia == referencia)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        // Insertar un nuevo DTE.
        public bool Insertar(Factura factura)
        {
            if (BuscarPorReferencia(factura.Referencia) != null)
            {
                return false;
            }

            NodoFactura nuevo = new NodoFactura(factura);

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

        // Obtener un DTE por su posicion.
        public Factura? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoFactura? actual = primero;
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

        // Contar DTE autorizados por NIT emisor.
        public int ContarEmitidos(string nit, DateTime fechaInicio, DateTime fechaFin)
        {
            int contador = 0;
            NodoFactura? actual = primero;

            while (actual != null)
            {
                Factura factura = actual.Dato;

                if (factura.Autorizada && factura.NitEmisor == nit &&
                    factura.Fecha.Date >= fechaInicio.Date && factura.Fecha.Date <= fechaFin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Contar DTE autorizados por NIT receptor.
        public int ContarRecibidos(string nit, DateTime fechaInicio, DateTime fechaFin)
        {
            int contador = 0;
            NodoFactura? actual = primero;

            while (actual != null)
            {
                Factura factura = actual.Dato;

                if (factura.Autorizada && factura.NitReceptor == nit &&
                    factura.Fecha.Date >= fechaInicio.Date && factura.Fecha.Date <= fechaFin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Calcular el monto total de DTE emitidos.
        public decimal TotalEmitido(string nit, DateTime fechaInicio, DateTime fechaFin)
        {
            decimal total = 0;
            NodoFactura? actual = primero;

            while (actual != null)
            {
                Factura factura = actual.Dato;

                if (factura.Autorizada && factura.NitEmisor == nit &&
                    factura.Fecha.Date >= fechaInicio.Date && factura .Fecha.Date <= fechaFin.Date)
                {
                    total += factura.Total;
                }

                actual = actual.Siguiente;
            }

            return total;
        }

        // Calcular el monto total de DTE recibidos.
        public decimal TotalRecibido(string nit, DateTime fechaInicio, DateTime fechaFin)
        {
            decimal total = 0;
            NodoFactura? actual = primero;

            while (actual != null)
            {
                Factura factura = actual.Dato;

                if (factura.Autorizada && factura.NitReceptor == nit &&
                    factura.Fecha.Date >= fechaInicio.Date && factura.Fecha.Date <= fechaFin.Date)
                {
                    total += factura.Total;
                }

                actual = actual.Siguiente;
            }

            return total;
        }

        // Eliminar todos los nodos.
        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
