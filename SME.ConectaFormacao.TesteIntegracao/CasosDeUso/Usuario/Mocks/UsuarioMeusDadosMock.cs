using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioMeusDadosMock
    {
        public static string Login { get; set; }

        public static string LoginNaoEncontrado { get; set; }

        public static DadosUsuarioDTO DadosUsuarioDTO { get; set; }

        public static void Montar()
        {
            var pessoa = new Person("pt_BR");

            Login = pessoa.FirstName;

            DadosUsuarioDTO = new DadosUsuarioDTO
            {
                Nome = pessoa.FullName,
                Login = pessoa.FirstName,
                Email = pessoa.Email,
                Cpf = pessoa.Cpf()
            };

            LoginNaoEncontrado = new Person("pt_BR").FirstName;
        }
    }
}
