
using System;
using System.Globalization;
using Backend.Validaciones;

namespace Backend.Ayuda
{
    public class ValidadorRango
    {
        public bool ValidarFecha(string? texto, out DateTime fecha)
        {
            return DateTime.TryParseExact(texto, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
        }

        public bool Validar(string? inicio, string? fin, out DateTime fechaInicio, out DateTime fechaFin)
        {
            fechaInicio = default;
            fechaFin = default;

            if (!ValidarFecha(inicio, out fechaInicio))
            {
                return false;
            }

            if (!ValidarFecha(fin, out fechaFin))
            {
                return false;
            }

            return fechaInicio.Date <= fechaFin.Date;
        }
    }
}
