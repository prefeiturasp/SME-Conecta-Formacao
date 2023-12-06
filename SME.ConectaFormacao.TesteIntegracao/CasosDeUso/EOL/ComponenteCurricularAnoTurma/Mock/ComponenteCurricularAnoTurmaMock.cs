using System.Collections;
using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma.Mock
{
    public static class ComponenteCurricularAnoTurmaMock
    {
        public static IEnumerable<ComponenteCurricularEOLDTO> ComponentesCurricularesAnosTurmas { get; set; }
        
        public static IEnumerable<ComponenteCurricularEOLDTO> GerarLista(int quantidade = 10)
        {
            var opcoesModalidades = new [] 
            { 
                Modalidade.Fundamental, 
                Modalidade.EducacaoInfantil,
                Modalidade.EJA,
                Modalidade.CIEJA,
                Modalidade.Medio,
                Modalidade.CMCT,
                Modalidade.MOVA,
                Modalidade.ETEC,
                Modalidade.CELP
            };

            var componenteCurricularAnoTurma = new List<ComponenteCurricularEOLDTO>();
            
            foreach (var modalidade in opcoesModalidades)
                componenteCurricularAnoTurma.AddRange(Gerador(modalidade, quantidade));
            
            ComponentesCurricularesAnosTurmas = componenteCurricularAnoTurma;

            return componenteCurricularAnoTurma;
        }

        private static IEnumerable<ComponenteCurricularEOLDTO> Gerador(Modalidade modalidade,int quantidade)
        {
            var codigoComponente = 1;
            var serieEnsino = 1000;
            var anoTurma = 0;

            var faker = new Faker<ComponenteCurricularEOLDTO>();

            faker.RuleFor(x => x.Codigo, f => codigoComponente++);
            faker.RuleFor(x => x.Descricao, f => f.Lorem.Text().Limite(70));
            faker.RuleFor(x => x.AnoTurma, f => anoTurma++.GerarAte(9));
            faker.RuleFor(x => x.SerieEnsino, f => $"{f.Random.Int(min: 1, max: 9)}º {f.Lorem.Slug()}");
            faker.RuleFor(x => x.CodigoSerieEnsino, f => serieEnsino++);
            faker.RuleFor(x => x.Modalidade, modalidade);

            return faker.Generate(quantidade);
        }
    }
}
