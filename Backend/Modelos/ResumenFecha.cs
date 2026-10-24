// Consultar el valor de los DTE autorizados en una fecha
using System;

namespace Backend.Modelos
{
    public class ResumenFecha
    {
        public DateTime Fecha { get; set; }

        public decimal TotalConIva { get; set; }

        public decimal TotalSinIva { get; set; }

        public int CantidadDTE { get; set; }

        public ResumenFecha(DateTime fecha)
        {
            Fecha = fecha.Date;
            TotalConIva = 0;
            TotalSinIva = 0;
            CantidadDTE = 0;
        }

        // Acumular los valores de un DTE autorizado.
        public void AgregarDTE(decimal valor, decimal total)
        {
            TotalSinIva += valor;
            TotalConIva += total;
            CantidadDTE++;
        }

        // Obtener el monto segun el tipo solicitado.
        public decimal ObtenerMonto(bool incluirIva)
        {
            if (incluirIva)
            {
                return TotalConIva;
            }

            return TotalSinIva;
        }
    }
}
