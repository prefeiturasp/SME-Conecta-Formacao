using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Proposta.Mocks
{
    public class RoteiroPropostaFormativaMock
    {
        public static IEnumerable<RoteiroPropostaFormativa> GerarRoteiroPropostaFormativa(int quantidade)
        {
            var faker = new Faker<RoteiroPropostaFormativa>();
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Sentence(3));
            faker.RuleFor(dest => dest.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(dest => dest.CriadoPor, f => f.Person.FullName);
            faker.RuleFor(dest => dest.CriadoLogin, f => f.Person.FirstName);
            return faker.Generate(quantidade);
        }
    }
}
