using SME.ConectaFormacao.Dominio.Extensoes;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Infra.Servicos.Utilitarios
{
    public static class UtilValidacoes
    {
        public static bool EmailEhValido(string email)
        {
            const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool EmailEduEhValido(string email)
        {
            const string pattern = @"^[a-zA-Z0-9._%+-]+@edu\.sme\.prefeitura\.sp\.gov\.br$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool CpfEhValido(string cpf)
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

        public static bool NomeComSobrenome(string nome)
        {
            return nome.Split(' ').Length > 1;
        }
    }
}