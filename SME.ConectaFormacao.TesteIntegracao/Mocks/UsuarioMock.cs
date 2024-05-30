using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class UsuarioMock : BaseMock
    {
        public static Usuario GerarUsuario(TipoUsuario tipoUsuario = TipoUsuario.Interno)
        {
            var faker = new Faker<Usuario>("pt_BR");
            faker.RuleFor(dest => dest.Login, f => f.Random.Long(10000, 99999).ToString());
            faker.RuleFor(dest => dest.Nome, f => f.Person.FullName);
            faker.RuleFor(dest => dest.Email, f => $"{f.Person.FirstName}@edu.sme.prefeitura.sp.gov.br");
            faker.RuleFor(dest => dest.Cpf, f => f.Person.Cpf(false));
            faker.RuleFor(dest => dest.Tipo, f => tipoUsuario);

            AuditoriaFaker(faker);

            return faker.Generate();
        }

        public static IEnumerable<Usuario> GerarUsuarios(AreaPromotora areaPromotora, TipoUsuario tipoUsuario = TipoUsuario.Interno, int quantidade = 1)
        {
            var faker = new Faker<Usuario>("pt_BR");
            faker.RuleFor(dest => dest.Login, f => f.Random.Long(10000, 99999).ToString());
            faker.RuleFor(dest => dest.Nome, f => f.Person.FullName);
            faker.RuleFor(dest => dest.Email, f => f.Person.Email);
            faker.RuleFor(dest => dest.Cpf, f => f.Person.Cpf(false));
            faker.RuleFor(dest => dest.Tipo, f => tipoUsuario);
            faker.RuleFor(dest => dest.AreaPromotoraId, f => areaPromotora.Id);
            faker.RuleFor(dest => dest.Telefone, f => f.Person.Phone.SomenteNumeros());
            faker.RuleFor(dest => dest.Situacao, f => f.PickRandom<SituacaoUsuario>());

            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }

        public static Faker<Usuario> GerarUsuarioFaker(TipoUsuario tipoUsuario = TipoUsuario.Interno)
        {
            var faker = new Faker<Usuario>("pt_BR");
            faker.RuleFor(dest => dest.Login, f => f.Random.Long(1000000, 9999999).ToString());
            faker.RuleFor(dest => dest.Nome, f => f.Person.FullName);
            faker.RuleFor(dest => dest.Email, f => f.Person.Email);
            faker.RuleFor(dest => dest.Cpf, f => f.Person.Cpf(false));
            faker.RuleFor(dest => dest.Tipo, f => tipoUsuario);

            AuditoriaFaker(faker);

            return faker;
        }
    }
}