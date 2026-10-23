
using Backend.Estructuras;

namespace Backend.Modelos
{
    public class ResultadoAutorizaciones
    {
        public int CantidadAutorizacionesAprobadas { get; set; }

        public int CantidadAutorizacionesRechazadasPorNitEmisorInexistente { get; set; }

        public int CantidadAutorizacionesRechazadasPorNitReceptorInexistente { get; set; }

        public ListaAprobaciones DetalleAprobaciones { get; set; }

        public ResultadoAutorizaciones()
        {
            CantidadAutorizacionesAprobadas = 0;

            CantidadAutorizacionesRechazadasPorNitEmisorInexistente = 0;

            CantidadAutorizacionesRechazadasPorNitReceptorInexistente = 0;

            DetalleAprobaciones = new ListaAprobaciones();
        }

        public void AgregarAprobacion(string referencia, long codigo)
        {
            DetalleAprobacion detalle = new DetalleAprobacion(referencia, codigo);

            DetalleAprobaciones.Insertar(detalle);

            CantidadAutorizacionesAprobadas++;
        }
    }
}
