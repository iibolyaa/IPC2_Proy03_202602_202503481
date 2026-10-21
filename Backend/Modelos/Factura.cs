
namespace Backend.Modelos
{
    public class Factura
    {
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; }
        public string NitEmisor { get; set; }
        public string NitReceptor { get; set; }

        public decimal Valor { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        
        public string CodigoAutorizacion { get; set; }
        public bool Autorizada { get; set; }

        public Factura(DateTime fecha, string referencia, string nitEmisor, string nitReceptor,
            decimal valor, decimal iva, decimal total)
        {
            Fecha = fecha;
            Referencia = referencia;
            NitEmisor = nitEmisor;
            NitReceptor = nitReceptor;

            Valor = valor;
            Iva = iva;
            Total = total;

            CodigoAutorizacion = "";
            Autorizada = false;
        }
    }
}
