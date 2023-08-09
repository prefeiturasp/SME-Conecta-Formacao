using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks
{
    public class UsuarioMock
    {
        public static string Login;

        public static string LoginNaoEncontrado;

        public static DadosUsuarioDTO DadosUsuarioDTO;

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
