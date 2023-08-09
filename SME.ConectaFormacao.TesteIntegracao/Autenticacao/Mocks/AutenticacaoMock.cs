using Bogus;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Autenticacao.Mocks
{
    public static class AutenticacaoMock
    {
        public static AutenticacaoDTO AutenticacaoUsuarioDTOValido;

        public static UsuarioAutenticacaoRetornoDTO UsuarioAutenticacaoRetornoDTOValido;

        public static UsuarioPerfisRetornoDTO UsuarioPerfisRetornoDTOValido;

        public static AutenticacaoDTO AutenticacaoUsuarioDTOInvalido;

        public static void Montar()
        {
            MontarAutenticacaoValida();
            MontarAutenticacaoInvalida();
        }

        private static void MontarAutenticacaoValida()
        {
            var faker = new Faker("pt_BR");

            AutenticacaoUsuarioDTOValido = new()
            {
                Login = faker.Person.FirstName,
                Senha = faker.Random.AlphaNumeric(10)
            };

            UsuarioAutenticacaoRetornoDTOValido = new()
            {
                Login = faker.Person.FirstName,
                Nome = faker.Person.FullName,
                Email = faker.Person.Email
            };

            UsuarioPerfisRetornoDTOValido = new()
            {
                UsuarioLogin = faker.Person.FirstName,
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
                Login = faker.Person.FirstName,
                Senha = new Faker().Random.AlphaNumeric(10)
            };
        }
    }
}
