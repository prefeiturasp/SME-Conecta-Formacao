using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class CriterioValidacaoInscricaoMock
    {
        private static Faker<CriterioValidacaoInscricao> Gerador(bool unico, bool outros)
        {
            var faker = new Faker<CriterioValidacaoInscricao>();
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Sentence(3));
            faker.RuleFor(dest => dest.Unico, unico);
            faker.RuleFor(dest => dest.Outros, outros);
            faker.RuleFor(dest => dest.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(dest => dest.CriadoPor, f => f.Person.FullName);
            faker.RuleFor(dest => dest.CriadoLogin, f => f.Person.FirstName);
            return faker;
        }

        public static IEnumerable<CriterioValidacaoInscricao> GerarCriterioValidacaoInscricao(int quantidade, bool unico = false, bool outros = false)
        {
            return Gerador(unico, outros).Generate(quantidade);
        }

        public static CriterioValidacaoInscricao GerarCriterioValidacaoInscricao(bool unico = false, bool outros = false)
        {
            return Gerador(unico, outros).Generate();
        }
    }
}
