
using Backend.Modelos;

namespace Backend.Estructuras
{
    public class ListaResumenIva
    {
        private NodoResumenIva? primero;
        private NodoResumenIva? ultimo;
        private int cantidad;

        public ListaResumenIva()
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

        // Buscar el resumen de un NIT.
        public ResumenIva? BuscarPorNit(string nit)
        {
            NodoResumenIva? actual = primero;

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

        // Obtener o crear el resumen correspondiente.
        private ResumenIva ObtenerOCrear(string nit)
        {
            ResumenIva? encontrado = BuscarPorNit(nit);

            if (encontrado != null)
            {
                return encontrado;
            }

            ResumenIva resumen = new ResumenIva(nit);

            NodoResumenIva nuevo =
                new NodoResumenIva(resumen);

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

            return resumen;
        }

        // Acumular IVA de documentos emitidos.
        public void AgregarEmitido(string nit, decimal iva)
        {
            ResumenIva resumen = ObtenerOCrear(nit);
            
            resumen.AgregarIvaEmitido(iva);
        }

        // Acumular IVA de documentos recibidos.
        public void AgregarRecibido(string nit, decimal iva)
        {
            ResumenIva resumen = ObtenerOCrear(nit);

            resumen.AgregarIvaRecibido(iva);
        }

        // Obtener un registro por posicion.
        public ResumenIva? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
            {
                return null;
            }

            NodoResumenIva? actual = primero;
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

        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }
    }
}
