using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class StringExtensao
    {
        public static string SomenteNumeros(this string valor)
        {
            return Regex.Replace(valor, "[^0-9]", "");
        }

        public static string AplicarMascara(this string valor, string mascara)
        {
            valor = valor.SomenteNumeros();
            if (string.IsNullOrEmpty(valor))
                return valor;

            return Convert.ToUInt64(valor).ToString(mascara);
        }

        public static string TratarEmail(this string email)
        {
            var inicioServidor = email.LastIndexOf('@');
            return string.Concat(email.AsSpan(0, 3), new string('*', inicioServidor - 3), email.AsSpan(inicioServidor));
        }

        public static bool EmailEhValido(this string email)
        {
            var regex = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$";
            return Regex.IsMatch(email, regex);
        }

        public static string Parametros(this string valor, params object[] parametros)
        {
            return string.Format(valor, parametros);
        }
    }
}
