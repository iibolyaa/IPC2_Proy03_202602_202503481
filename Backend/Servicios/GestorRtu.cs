using System;
using Backend.Modelos;
using Backend.Estructuras;
using Backend.Validaciones;

namespace Backend.Ayuda
{
    public class GestorRtu
    {
        private readonly ListaContribuyentes contribuyentes;
        private readonly ValidadorNit validadorNit;

        public GestorRtu(ListaContribuyentes contribuyentes)
        {
            this.contribuyentes = contribuyentes;
            validadorNit = new ValidadorNit();
        }

        // Procesar un contribuyente recibido.
        public void ProcesarContribuyente(string? nit, string? nombre, RespuestaRtu respuesta)
        {
            // Verificar que el NIT sea valido.
            if (!validadorNit.Validar(nit))
            {
                respuesta.NitsInvalidos++;
                return;
            }

            // El NIT ya se verifico.
            string nitValido = nit!;

            // Buscar el NIT dentro del RTU.
            Contribuyente? existente = contribuyentes.Buscar(nitValido);

            if (existente == null)
            {
                // Registrar un contribuyente nuevo.
                Contribuyente nuevo = new Contribuyente(nitValido, nombre ?? "");

                bool insertado = contribuyentes.Insertar(nuevo);

                if (insertado)
                {
                    respuesta.ContribuyentesNuevos++;
                }
            }
            else
            {
                // Actualizar el contribuyente existente.
                contribuyentes.Actualizar(nitValido, nombre ?? "");

                respuesta.ContribuyentesActualizados++;
            }
        }

        // Buscar un contribuyente por NIT.
        public Contribuyente? Buscar(string nit)
        {
            return contribuyentes.Buscar(nit);
        }

        // Consultar cuantos contribuyentes existen.
        public int ObtenerCantidad()
        {
            return contribuyentes.ObtenerCantidad();
        }

        // Obtener un contribuyente por posicion.
        public Contribuyente? ObtenerPorIndice(int indice)
        {
            return contribuyentes.ObtenerPorIndice(indice);
        }

        // Limpiar los contribuyentes en memoria.
        public void Limpiar()
        {
            contribuyentes.Limpiar();
        }
    }
}
