
using System;
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaSolicitudes
    {
        private NodoSolicitud? primero;
        private NodoSolicitud? ultimo;
        private int cantidad;

        public ListaSolicitudes()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        public bool EstaVacia()
        {
            return primero == null;
        }

        public int ObtenerCantidad()
        {
            return cantidad;
        }

        public void Insertar(SolicitudProcesada solicitud)
        {
            NodoSolicitud nuevo = new NodoSolicitud(solicitud);

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

        public SolicitudProcesada? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoSolicitud? actual = primero;
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

        // Contar solicitudes dentro del rango indicado
        public int ContarPorRango(DateTime inicio, DateTime fin)
        {
            int contador = 0;
            NodoSolicitud? actual = primero;

            while (actual != null)
            {
                DateTime fecha = actual.Dato.FechaDTE ?? actual.Dato.FechaRecepcion;

                if (fecha.Date >= inicio.Date && fecha.Date <= fin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Contar solicitudes aprobadas
        public int ContarAprobadas(DateTime inicio, DateTime fin)
        {
            int contador = 0;
            NodoSolicitud? actual = primero;

            while (actual != null)
            {
                SolicitudProcesada solicitud = actual.Dato;

                DateTime fecha = solicitud.FechaDTE ?? solicitud.FechaRecepcion;

                if (solicitud.Aprobada && fecha.Date >= inicio.Date && fecha.Date <= fin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Contar solicitudes rechazadas.
        public int ContarRechazadas(DateTime inicio, DateTime fin)
        {
            int contador = 0;
            NodoSolicitud? actual = primero;

            while (actual != null)
            {
                SolicitudProcesada solicitud = actual.Dato;

                DateTime fecha = solicitud.FechaDTE ?? solicitud.FechaRecepcion;

                if (!solicitud.Aprobada && fecha.Date >= inicio.Date && fecha.Date <= fin.Date)
                {
                    contador++;
                }

                actual = actual.Siguiente;
            }

            return contador;
        }

        // Comprobar si ya se utilizo una referencia
        public bool ExisteReferencia(string referencia)
        {
            NodoSolicitud? actual = primero;

            while (actual != null)
            {
                if (actual.Dato.Referencia == referencia)
                {
                    return true;
                }

                actual = actual.Siguiente;
            }

            return false;
        }

        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
