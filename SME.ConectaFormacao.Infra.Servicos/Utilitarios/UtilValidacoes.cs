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
            // Aloca espaço na stack para armazenar apenas os números, evitando Garbage Collector
            Span<int> numeros = stackalloc int[11];
            int quantidade = 0;

            // Itera sobre o input extraindo apenas números
            foreach (char c in cpf)
            {
                if (char.IsDigit(c))
                {
                    if (quantidade < 11)
                    {
                        numeros[quantidade++] = c - '0';
                    }
                    else
                    {
                        return false; // Mais de 11 dígitos
                    }
                }
            }

            // Validações básicas de tamanho e todos dígitos iguais
            if (quantidade != 11) return false;

            bool todosIguais = true;
            for (int i = 1; i < 11; i++)
            {
                if (numeros[i] != numeros[0])
                {
                    todosIguais = false;
                    break;
                }
            }
            if (todosIguais) return false;

            // Cálculo do primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += numeros[i] * (10 - i);

            int resto = soma % 11;
            int primeiroDigitoCalculado = resto < 2 ? 0 : 11 - resto;

            if (numeros[9] != primeiroDigitoCalculado) return false;

            // Cálculo do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += numeros[i] * (11 - i);

            resto = soma % 11;
            int segundoDigitoCalculado = resto < 2 ? 0 : 11 - resto;

            return numeros[10] == segundoDigitoCalculado;
        }

        public static bool NomeComSobrenome(string nome)
        {
            return nome.Split(' ').Length > 1;
        }
    }
}