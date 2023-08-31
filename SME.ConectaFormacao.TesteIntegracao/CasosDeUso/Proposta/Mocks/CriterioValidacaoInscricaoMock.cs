using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class CriterioValidacaoInscricaoMock
    {
        public static IEnumerable<CriterioValidacaoInscricao> GerarCriterioValidacaoInscricao(int quantidade)
        {
            var faker = new Faker<CriterioValidacaoInscricao>();
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Sentence(3));
            faker.RuleFor(dest => dest.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(dest => dest.CriadoPor, f => f.Person.FullName);
            faker.RuleFor(dest => dest.CriadoLogin, f => f.Person.FirstName);
            return faker.Generate(quantidade);
        }
    }
}
