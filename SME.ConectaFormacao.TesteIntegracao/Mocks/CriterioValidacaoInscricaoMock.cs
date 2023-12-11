using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class CriterioValidacaoInscricaoMock : BaseMock
    {
        private static Faker<CriterioValidacaoInscricao> Gerador(bool unico, bool outros)
        {
            var faker = new Faker<CriterioValidacaoInscricao>();
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Sentence(3));
            faker.RuleFor(dest => dest.Unico, unico);
            faker.RuleFor(dest => dest.Outros, outros);
            AuditoriaFaker(faker);
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
