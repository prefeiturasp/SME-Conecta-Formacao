using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock
{
    public class AreaPromotoraMock
    {

        private static Faker<Dominio.Entidades.AreaPromotora> Gerador()
        {
            var faker = new Faker<Dominio.Entidades.AreaPromotora>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            return faker;
        }

        public static Dominio.Entidades.AreaPromotora GerarAreaPromotora()
        {
            return Gerador().Generate();
        }

        public static IEnumerable<Dominio.Entidades.AreaPromotora> GerarAreaPromotora(int quantidade)
        {
            return Gerador().Generate(quantidade);
        }

        public static Dominio.Entidades.AreaPromotoraTelefone GerarAreaTelefone(long areaPromotoraId, string telefone = null)
        {
            var faker = new Faker<Dominio.Entidades.AreaPromotoraTelefone>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Telefone, f => string.IsNullOrEmpty(telefone) ? f.Phone.PhoneNumber("###########") : telefone.SomenteNumeros());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            return faker.Generate();
        }
    }
}
