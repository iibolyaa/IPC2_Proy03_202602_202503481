
namespace Backend.Modelos
{
    public class ErrorFactura
    {
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; }
        public string TipoError { get; set; }
        public string Descripcion { get; set; }

        public ErrorFactura(DateTime fecha, string referencia, string tipoError, string descripcion)
        {
            Fecha = fecha;
            Referencia = referencia;
            TipoError = tipoError;
            Descripcion = descripcion;
        }
    }
}