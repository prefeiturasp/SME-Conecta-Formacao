using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class UsuarioMock : BaseMock
    {
        public static Usuario GerarUsuario()
        {
            var faker = new Faker<Usuario>();
            faker.RuleFor(dest => dest.Login, f => f.Random.Long(10000, 99999).ToString());
            faker.RuleFor(dest => dest.Nome, f => f.Person.FullName);
            faker.RuleFor(dest => dest.Email, f => f.Person.Email);

            AuditoriaFaker(faker);

            return faker.Generate();
        }
    }
}