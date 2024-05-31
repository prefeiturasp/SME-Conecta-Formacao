using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class CriterioValidacaoInscricaoMock : BaseMock
    {
        private static Faker<CriterioValidacaoInscricao> Gerador(bool unico, bool outros, bool permiteSorteio)
        {
            var faker = new Faker<CriterioValidacaoInscricao>();
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Sentence(3));
            faker.RuleFor(dest => dest.Unico, unico);
            faker.RuleFor(dest => dest.Outros, outros);
            faker.RuleFor(dest => dest.PermiteSorteio, permiteSorteio);
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<CriterioValidacaoInscricao> GerarCriterioValidacaoInscricao(int quantidade, bool unico = false, bool outros = false, bool permiteSorteio = false)
        {
            return Gerador(unico, outros, permiteSorteio).Generate(quantidade);
        }

        public static CriterioValidacaoInscricao GerarCriterioValidacaoInscricao(bool unico = false, bool outros = false, bool permiteSorteio = false)
        {
            return Gerador(unico, outros, permiteSorteio).Generate();
        }
    }
}
