using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class ComponenteCurricularMock : BaseMock
    {
        private static Faker<ComponenteCurricular> Gerador(bool todos = false)
        {
            var codigoEol = 1;
            var faker = new Faker<ComponenteCurricular>();
            faker.RuleFor(dest => dest.CodigoEOL, f => codigoEol++);
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Text().Limite(70));
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<ComponenteCurricular> GerarComponenteCurricular(int quantidade, bool todos = false)
        {
            return Gerador(todos).Generate(quantidade);
        }

        public static ComponenteCurricular GerarComponenteCurricular(bool todos)
        {
            return Gerador(todos).Generate();
        }
    }
}
