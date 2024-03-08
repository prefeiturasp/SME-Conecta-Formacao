using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioAlterarUnidadeEolMock
    {
        public static string Login { get; set; }
        public static string UnidadeEol { get; set; }

        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;
            UnidadeEol = new Randomizer().Number(8).ToString();
        }
    }
}
