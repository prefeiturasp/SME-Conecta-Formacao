using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioAlterarEmailValidacaoMock
    {
        public static string Login { get; set; }
        public static string Senha { get; set; }
        public static string Email { get; set; }
        
        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;
            Email = pessoa.Email;
            Senha = new Randomizer().Number(8).ToString();
        }
    }
}