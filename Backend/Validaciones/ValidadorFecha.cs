
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Backend.Validaciones
{
    public class ValidadorFecha
    {
        // Expresion regular que identifica una fecha y hora.
        private readonly Regex expresionFecha;

        public ValidadorFecha()
        {
            expresionFecha = new Regex(
                @"(?<!\d)(?<fecha>\d{2}/\d{2}/\d{4})\s+(?<hora>(?:[01]\d|2[0-3]):[0-5]\d)(?!\d)",
                RegexOptions.Compiled
            );
        }

        // Extraer y validar la fecha del texto recibido.
        public bool IntentarObtenerFecha(string? texto, out DateTime fecha)
        {
            fecha = default;

            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            Match coincidencia = expresionFecha.Match(texto);

            if (!coincidencia.Success)
            {
                return false;
            }

            string fechaTexto =
                coincidencia.Groups["fecha"].Value;

            string horaTexto =
                coincidencia.Groups["hora"].Value;

            string fechaCompleta =
                fechaTexto + " " + horaTexto;

            return DateTime.TryParseExact(fechaCompleta, "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
        }

        // Comprobar si el texto contiene una fecha valida.
        public bool Validar(string? texto)
        {
            return IntentarObtenerFecha(texto, out _);
        }

        // Comprobar si un rango de fechas es valido.
        public bool ValidarRango(DateTime fechaInicio, DateTime fechaFin)
        {
            return fechaInicio.Date <= fechaFin.Date;
        }
    }
}
