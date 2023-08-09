using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks
{
    public class UsuarioAlterarEmailMock
    {
        public static string Login;

        public static string EmailValido;

        public static string EmailInvalido;

        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;
            EmailValido = pessoa.Email;
            EmailInvalido = pessoa.FullName;
        }
    }
}
