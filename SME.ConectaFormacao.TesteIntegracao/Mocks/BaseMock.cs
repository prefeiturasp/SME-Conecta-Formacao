using Bogus;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public abstract class BaseMock
    {
        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Long(1);
        }

        protected static void AuditoriaFaker<T>(Faker<T> faker) where T : EntidadeBaseAuditavel
        {
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            faker.RuleFor(x => x.AlteradoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.AlteradoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AlteradoLogin, f => f.Name.FirstName());
        }

        protected static void Auditoria<T>(T entidade) where T : EntidadeBaseAuditavel
        {
            var faker = new Faker("pt_BR");

            entidade.CriadoPor = faker.Name.FullName();
            entidade.CriadoEm = DateTimeExtension.HorarioBrasilia();
            entidade.CriadoLogin = faker.Name.FirstName();
            entidade.AlteradoPor = faker.Name.FullName();
            entidade.AlteradoEm = DateTimeExtension.HorarioBrasilia();
            entidade.AlteradoLogin = faker.Name.FirstName();
        }
    }
}
