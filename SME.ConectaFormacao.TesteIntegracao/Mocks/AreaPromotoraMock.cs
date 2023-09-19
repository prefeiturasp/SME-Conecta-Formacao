using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class AreaPromotoraMock
    {

        private static Faker<AreaPromotora> Gerador(Guid? grupoId = null)
        {
            var faker = new Faker<AreaPromotora>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Email, f => f.Person.Email);
            faker.RuleFor(x => x.GrupoId, grupoId.HasValue ? grupoId : Guid.NewGuid());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            faker.RuleFor(x => x.AlteradoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.AlteradoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AlteradoLogin, f => f.Name.FirstName());
            return faker;
        }

        private static Faker<AreaPromotoraTelefone> GeradorTelefone(long areaPromotoraId, string telefone)
        {
            var faker = new Faker<AreaPromotoraTelefone>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Telefone, f => string.IsNullOrEmpty(telefone) ? f.Phone.PhoneNumber("###########") : telefone.SomenteNumeros());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            return faker;
        }

        public static AreaPromotora GerarAreaPromotora(Guid? grupoId = null)
        {
            return Gerador(grupoId).Generate();
        }

        public static IEnumerable<AreaPromotora> GerarAreaPromotora(int quantidade, Guid? grupoId = null)
        {
            return Gerador(grupoId).Generate(quantidade);
        }

        public static AreaPromotoraTelefone GerarAreaTelefone(long areaPromotoraId, string telefone = null)
        {
            return GeradorTelefone(areaPromotoraId, telefone).Generate();
        }

        public static IEnumerable<AreaPromotoraTelefone> GerarAreaTelefone(int quantidade, long areaPromotoraId, string telefone = null)
        {
            var faker = new Faker<AreaPromotoraTelefone>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Telefone, f => string.IsNullOrEmpty(telefone) ? f.Phone.PhoneNumber("###########") : telefone.SomenteNumeros());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            return faker.Generate(quantidade);
        }
    }
}
