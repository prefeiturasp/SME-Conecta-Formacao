using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks
{
    public static class AutenticacaoMock
    {
        public static AutenticacaoDTO AutenticacaoUsuarioDTOValido { get; set; }

        public static UsuarioAutenticacaoRetornoDTO UsuarioAutenticacaoRetornoDTOValido { get; set; }

        public static UsuarioPerfisRetornoDTO UsuarioPerfisRetornoDTOValido { get; set; }

        public static AutenticacaoDTO AutenticacaoUsuarioDTOInvalido { get; set; }

        public static Dominio.Entidades.Usuario UsuarioLogado { get; set; }

        public static void Montar()
        {
            MontarAutenticacaoValida();
            MontarAutenticacaoInvalida();
            MontarUsuarioAutenticado();
        }

        private static void MontarAutenticacaoValida()
        {
            var faker = new Faker("pt_BR");

            AutenticacaoUsuarioDTOValido = new()
            {
                Login = faker.Random.Number(1000000, 9999999).ToString(),
                Senha = faker.Random.AlphaNumeric(10)
            };

            UsuarioAutenticacaoRetornoDTOValido = new()
            {
                Login = AutenticacaoUsuarioDTOValido.Login,
                Nome = faker.Person.FullName,
                Email = faker.Person.Email
            };

            UsuarioPerfisRetornoDTOValido = new()
            {
                UsuarioLogin = AutenticacaoUsuarioDTOValido.Login,
                UsuarioNome = faker.Person.FullName,
                Email = faker.Person.Email,
                Token = faker.Random.AlphaNumeric(100),
                DataHoraExpiracao = DateTimeExtension.HorarioBrasilia().AddMinutes(30)
            };
        }

        private static void MontarAutenticacaoInvalida()
        {
            var faker = new Faker("pt_BR");

            AutenticacaoUsuarioDTOInvalido = new()
            {
                Login = faker.Random.Number(1000000, 9999999).ToString(),
                Senha = new Faker().Random.AlphaNumeric(10)
            };
        }

        public static void MontarUsuarioAutenticado()
        {
            var faker = new Faker<Dominio.Entidades.Usuario>("pt_BR");

            faker.RuleFor(x => x.Login, f => f.Random.Number(1000000, 9999999).ToString());
            faker.RuleFor(x => x.Nome, f => f.Person.FullName);
            faker.RuleFor(x => x.Email, f => f.Person.Email);

            UsuarioLogado = faker.Generate();
        }
    }
}
