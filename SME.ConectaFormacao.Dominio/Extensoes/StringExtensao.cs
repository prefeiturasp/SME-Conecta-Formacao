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
    }
}
