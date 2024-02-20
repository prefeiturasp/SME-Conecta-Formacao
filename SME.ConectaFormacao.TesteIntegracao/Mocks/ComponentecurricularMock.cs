using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class ComponenteCurricularMock : BaseMock
    {
        private static Faker<ComponenteCurricular> Gerador(long? anoTurmaId, bool todos = false)
        {
            var codigoEol = 1;
            var faker = new Faker<ComponenteCurricular>();
            faker.RuleFor(dest => dest.CodigoEOL, f => codigoEol++);
            faker.RuleFor(dest => dest.Nome, f => f.Lorem.Text().Limite(70));
            faker.RuleFor(dest => dest.Todos, todos);
            faker.RuleFor(dest => dest.AnoTurmaId, anoTurmaId);
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<ComponenteCurricular> GerarComponenteCurricular(int quantidade, long anoTurmaId)
        {
            var retorno = new List<ComponenteCurricular>();

            retorno.AddRange(Gerador(anoTurmaId, false).Generate(quantidade));
            retorno.Add(Gerador(null, true).Generate());

            return retorno;
        }
    }
}
