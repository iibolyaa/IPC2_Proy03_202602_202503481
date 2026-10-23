using System;

namespace Backend.Modelos
{
    public class SolicitudProcesada
    {
        public DateTime FechaRecepcion { get; set; }
        public DateTime? FechaDTE { get; set; }
        public string Referencia { get; set; }
        public bool Aprobada { get; set; }

        public SolicitudProcesada(DateTime fechaRecepcion, DateTime? fechaDTE, string referencia, 
            bool aprobada)
        {
            FechaRecepcion = fechaRecepcion;
            FechaDTE = fechaDTE;
            Referencia = referencia;
            Aprobada = aprobada;
        }
    }
}