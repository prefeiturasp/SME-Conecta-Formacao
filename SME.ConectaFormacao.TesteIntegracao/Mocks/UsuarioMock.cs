using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class UsuarioMock : BaseMock
    {
        public static Usuario GerarUsuario(TipoUsuario tipoUsuario = TipoUsuario.Interno)
        {
            var faker = new Faker<Usuario>("pt_BR");
            faker.RuleFor(dest => dest.Login, f => f.Random.Long(10000, 99999).ToString());
            faker.RuleFor(dest => dest.Nome, f => f.Person.FullName);
            faker.RuleFor(dest => dest.Email, f => f.Person.Email);
            faker.RuleFor(dest => dest.Cpf, f => f.Person.Cpf(false));
            faker.RuleFor(dest => dest.Tipo, f => tipoUsuario);

            AuditoriaFaker(faker);

            return faker.Generate();
        }
    }
}