//Almacena el IVA emitido y recibido por cada contribuyente
namespace Backend.Modelos
{
    public class ResumenIva
    {
        public string Nit { get; set; }

        public decimal IvaEmitido { get; set; }

        public decimal IvaRecibido { get; set; }

        public ResumenIva(string nit)
        {
            Nit = nit;
            IvaEmitido = 0;
            IvaRecibido = 0;
        }

        // Acumular IVA de DTE emitidos
        public void AgregarIvaEmitido(decimal iva)
        {
            IvaEmitido += iva;
        }

        // Acumular IVA de DTE recibidos
        public void AgregarIvaRecibido(decimal iva)
        {
            IvaRecibido += iva;
        }
    }
}
