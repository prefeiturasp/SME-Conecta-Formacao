using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class StringExtensao
    {
        public static readonly Regex RegexTagsBR = new("<br[^>]*>", RegexOptions.Compiled);
        public static readonly Regex RegexTagsP = new("<p[^>]*>", RegexOptions.Compiled);
        public static readonly Regex RegexTagsLI = new("<li[^>]*>", RegexOptions.Compiled);
        public static readonly Regex RegexTagsDIV = new("<div[^>]*>", RegexOptions.Compiled);
        public static readonly Regex RegexTagsHTMLQualquer = new("<[^>]*>", RegexOptions.Compiled);
        public static readonly Regex RegexEspacosEmBranco = new("&nbsp;", RegexOptions.Compiled);

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
            return tamanhoString > limite ? str.Substring(0, limite) : str;
        }

        public static bool EstaPreenchido(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool NaoEstaPreenchido(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Parametros(this string valor, params object[] parametros)
        {
            return string.Format(valor, parametros);
        }

        public static string RemoverTagsHtml(this string texto)
        {
            if (texto.NaoEstaPreenchido())
                return string.Empty;

            texto = RegexTagsBR.Replace(texto, " ");
            texto = RegexTagsP.Replace(texto, " ");
            texto = RegexTagsLI.Replace(texto, " ");
            texto = RegexTagsDIV.Replace(texto, " ");
            texto = RegexTagsHTMLQualquer.Replace(texto, string.Empty);
            texto = RegexEspacosEmBranco.Replace(texto, " ").Trim();
            return texto.Trim();
        }
        
        public static string RemoverAcentosECaracteresEspeciais(this string str)
        {
            var strResultado = str;
            string[] acentos = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < acentos.Length; i++)
            {
                strResultado = strResultado.Replace(acentos[i], semAcento[i]);
            }

            string[] caracteresEspeciais = { "\\.", ",", "-", ":", "\\(", "\\)", "ª", "\\|", "\\\\", "°" };

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                strResultado = strResultado.Replace(caracteresEspeciais[i], "");
            }

            strResultado = strResultado.Replace("^\\s+", "");
            strResultado = strResultado.Replace("\\s+$", "");
            strResultado = strResultado.Replace("\\s+", " ");
            return strResultado;
        }

        public static bool CpfEhValido(this string cpf)
        {
            cpf = cpf.SomenteNumeros();
            if (cpf.Length != 11)
                return false;

            char primeiroDigito = cpf[0];
            if (cpf == string.Empty.PadLeft(11, primeiroDigito))
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf, digito;
            int soma = 0, resto;
            tempCpf = cpf[..9];

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf += digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito += resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}
