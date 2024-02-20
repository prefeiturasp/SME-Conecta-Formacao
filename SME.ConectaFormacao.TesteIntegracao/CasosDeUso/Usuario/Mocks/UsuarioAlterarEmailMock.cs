using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioAlterarEmailMock
    {
        public static string Login { get; set; }

        public static string EmailValido { get; set; }

        public static string EmailInvalido { get; set; }

        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;
            EmailValido = pessoa.Email;
            EmailInvalido = pessoa.FullName;
        }
    }
}
