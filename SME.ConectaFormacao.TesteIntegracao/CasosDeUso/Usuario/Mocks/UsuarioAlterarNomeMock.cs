using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioAlterarNomeMock
    {
        public static string Login { get; set; }

        public static string NomeValido { get; set; }

        public static string NomeInvalido { get; set; }

        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;
            NomeValido = pessoa.FullName.ToUpper();
            NomeInvalido = string.Empty;
        }
    }
}
