using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioRecuperarSenhaMock
    {
        public static string LoginInvalido { get; set; }

        public static string LoginValido { get; set; }
        public static string EmailValido { get; set; }
        public static UsuarioPerfisRetornoDTO UsuarioPerfisRetornoDTOValido { get; set; }

        public static Guid TokenInvalido { get; set; }
        public static Guid TokenValido { get; set; }

        public static RecuperacaoSenhaDto RecuperacaoSenhaDto { get; set; }

        public static void Montar()
        {
            LoginInvalido = new Person("pt_BR").FirstName;

            var pessoa = new Person("pt_BR");

            LoginValido = pessoa.FirstName;
            EmailValido = pessoa.Email;

            UsuarioPerfisRetornoDTOValido = new()
            {
                UsuarioLogin = pessoa.FirstName,
                UsuarioNome = pessoa.FullName,
                Email = pessoa.Email,
                Token = new Faker().Random.AlphaNumeric(100),
                DataHoraExpiracao = DateTimeExtension.HorarioBrasilia().AddMinutes(30)
            };

            TokenInvalido = Guid.NewGuid();
            TokenValido = Guid.NewGuid();

            RecuperacaoSenhaDto = new RecuperacaoSenhaDto
            {
                Token = Guid.NewGuid(),
                NovaSenha = new Faker().Random.AlphaNumeric(12)
            };
        }
    }
}
