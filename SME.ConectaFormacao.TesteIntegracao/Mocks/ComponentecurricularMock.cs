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

        public static IEnumerable<ComponenteCurricular> GerarComponentesCurriculares(int quantidade, bool todos = false)
        {
            return Gerador(todos).Generate(quantidade);
        }
        
        public static IEnumerable<ComponenteCurricular> GerarComponentesCurricularesComAnoTurma(int quantidade, IEnumerable<AnoTurma> anosTurma,bool todos = false)
        {
            var componentesCurricularesComAnoTurma = new List<ComponenteCurricular>();
            
            foreach (var anoTurma in anosTurma)
            {
                var componentesCurriculares = Gerador(todos).Generate(quantidade);
               
                foreach (var componenteCurricular in componentesCurriculares)
                {
                    componenteCurricular.AnoTurmaId = anoTurma.Id;
                    componentesCurricularesComAnoTurma.Add(componenteCurricular);
                }
            }
            return componentesCurricularesComAnoTurma;
        }

        public static ComponenteCurricular GerarComponenteCurricular(bool todos)
        {
            return Gerador(todos).Generate();
        }
    }
}
