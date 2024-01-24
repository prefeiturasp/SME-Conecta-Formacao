using Bogus;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma.Mock
{
    public static class ComponenteCurricularAnoTurmaMock
    {
        public static IEnumerable<ComponenteCurricularAnoTurmaServicoEol> ComponentesCurricularesAnosTurmas { get; set; }

        public static IEnumerable<ComponenteCurricularAnoTurmaServicoEol> GerarLista(Dominio.Enumerados.Modalidade modalidade = Dominio.Enumerados.Modalidade.Fundamental, int quantidade = 10)
        {
            ComponentesCurricularesAnosTurmas = Gerador(modalidade, quantidade);

            return ComponentesCurricularesAnosTurmas;
        }

        private static IEnumerable<ComponenteCurricularAnoTurmaServicoEol> Gerador(Dominio.Enumerados.Modalidade modalidade, int quantidade)
        {
            var codigoComponente = 1;
            var serieEnsino = 1000;
            var anoTurma = 0;

            var faker = new Faker<ComponenteCurricularAnoTurmaServicoEol>();

            faker.RuleFor(x => x.CodigoComponenteCurricular, f => codigoComponente++);
            faker.RuleFor(x => x.DescricaoComponenteCurricular, f => f.Lorem.Text().Limite(70));
            faker.RuleFor(x => x.CodigoAnoTurma, f => anoTurma++.GerarAte(9));
            faker.RuleFor(x => x.DescricaoSerieEnsino, f => $"{f.Random.Int(min: 1, max: 9)}º {f.Lorem.Slug()}");
            faker.RuleFor(x => x.CodigoSerieEnsino, f => serieEnsino++);
            faker.RuleFor(x => x.Modalidade, modalidade);

            return faker.Generate(quantidade);
        }
    }
}
