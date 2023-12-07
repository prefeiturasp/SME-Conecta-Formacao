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
        
        public static string Limite(this string str, int limite)
        {
            var tamanhoString = str.Length;
            return tamanhoString > limite ? str.Substring(0,limite) : str;
        }
        
        public static bool EstaPreenchido(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        
        public static bool NaoEstaPreenchido(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
