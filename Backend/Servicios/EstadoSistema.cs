
using System;
using Backend.Archivos;
using Backend.Estructuras;

namespace Backend.Ayuda
{
    public class EstadoSistema
    {
        // Estructuras enlazadas del sistema
        public ListaContribuyentes Contribuyentes { get; }
        public ListaFacturas Documentos { get; }
        public ListaErrores Errores { get; }
        public ListaCorrelativos Correlativos { get; }
        public ListaSolicitudes Solicitudes { get; }

        // Administrador de archivos.
        private readonly Persistencia persistencia;

        // Bloqueo compartido para las operaciones.
        private readonly object bloqueo;

        public EstadoSistema(string carpetaBackend)
        {
            bloqueo = new object();

            // Crear las estructuras propias
            Contribuyentes = new ListaContribuyentes();
            Documentos = new ListaFacturas();
            Errores = new ListaErrores();
            Correlativos = new ListaCorrelativos();
            Solicitudes = new ListaSolicitudes();

            // Crear el administrador de archivos
            persistencia = new Persistencia(carpetaBackend);

            // Recuperar los datos anteriores
            persistencia.Cargar(Contribuyentes, Documentos, Errores, Correlativos, Solicitudes);
        }

        // Ejecutar una consulta protegida
        public T Consultar<T>(Func<T> consulta)
        {
            lock (bloqueo)
            {
                return consulta();
            }
        }

        // Modificar estructuras y guardar los cambios
        public void EjecutarCambio(Action operacion)
        {
            lock (bloqueo)
            {
                // Ejecutar la operacion sobre las listas.
                operacion();

                // Guardar las cuatro estructuras.
                persistencia.Guardar(Contribuyentes, Documentos, Errores, Correlativos, Solicitudes);
            }
        }

        // Reiniciar el sistema
        public void Inicializar()
        {
            lock (bloqueo)
            {
                // Vaciar las cuatro estructuras.
                Contribuyentes.Limpiar();
                Documentos.Limpiar();
                Errores.Limpiar();
                Correlativos.Limpiar();
                Solicitudes.Limpiar();

                // Guardar el estado vacio.
                persistencia.Guardar(Contribuyentes, Documentos, Errores, Correlativos, Solicitudes);
            }
        }
    }
}
