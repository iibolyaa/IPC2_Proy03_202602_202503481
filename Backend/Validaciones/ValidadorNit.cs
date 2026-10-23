
using System;
using System.Text.RegularExpressions;

namespace Backend.Validaciones
{
    public class ValidadorNit
    {
        // Expresion regular para validar el formato.
        // Entre 1 y 20 digitos base y un verificador numerico o la letra K.
        private readonly Regex formatoNit;

        public ValidadorNit()
        {
            formatoNit = new Regex(@"^[0-9]{1,20}[0-9K]$");
        }

        // Validar que el NIT tenga el formato permitido.
        public bool ValidarFormato(string? nit)
        {
            if (string.IsNullOrEmpty(nit))
            {
                return false;
            }

            return formatoNit.IsMatch(nit);
        }

        // Calcular el caracter verificador del NIT.
        public char CalcularVerificador(string numeroBase)
        {
            int suma = 0;
            int factor = 2;

            // Recorrer los digitos de derecha a izquierda.
            for (int i = numeroBase.Length - 1; i >= 0; i--)
            {
                int digito = numeroBase[i] - '0';

                suma += digito * factor;

                factor++;
            }

            // Aplicar el algoritmo de modulo 11.
            int residuo = suma % 11;

            int resultado = (11 - residuo) % 11;

            // El valor 10 se representa con K.
            if (resultado == 10)
            {
                return 'K';
            }

            // Convertir el resultado numerico a caracter.
            return (char)('0' + resultado);
        }

        // Validar completamente el NIT.
        public bool Validar(string? nit)
        {
            // Primero validar el formato.
            if (!ValidarFormato(nit))
            {
                return false;
            }

            // En este punto nit no es nulo.
            string nitCompleto = nit!;

            // Separar el numero base del verificador.
            string numeroBase = nitCompleto.Substring(0, nitCompleto.Length - 1);

            char verificadorIngresado = nitCompleto[nitCompleto.Length - 1];

            // Calcular el verificador correcto.
            char verificadorCalculado = CalcularVerificador(numeroBase);

            // Comparar los verificadores.
            return verificadorIngresado == verificadorCalculado;
        }
    }
}
