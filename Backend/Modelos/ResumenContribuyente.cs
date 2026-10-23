
using System;

namespace Backend.Modelos
{
    public class ResumenContribuyente
    {
        public DateTime Fecha { get; set; }

        public int CantidadDTE { get; set; }

        public decimal MontoTotal { get; set; }

        public ResumenContribuyente(DateTime fecha)
        {
            Fecha = fecha.Date;
            CantidadDTE = 0;
            MontoTotal = 0;
        }

        // Agregar un DTE al resumen.
        public void AgregarDTE(decimal monto)
        {
            CantidadDTE++;
            MontoTotal += monto;
        }
    }
}
