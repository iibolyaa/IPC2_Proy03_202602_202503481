
namespace Backend.Modelos
{
    public class Correlativo
    {
        public DateTime Fecha { get; set; }
        public int UltimoNumero { get; set; }

        public Correlativo(DateTime fecha, int ultimoNumero)
        {
            Fecha = fecha.Date;
            UltimoNumero = ultimoNumero;
        }
    }
}
