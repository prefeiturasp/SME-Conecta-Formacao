using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks
{
    public static class AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoMock
    {
        public static ComponenteCurricularEAnoTurmaFiltrosDTO ComponenteCurricularEAnoTurmaFiltrosDto { get; set; }

        public static void Montar(Modalidade modalidade, bool todos = false)
        {
            var faker = new Faker<ComponenteCurricularEAnoTurmaFiltrosDTO>("pt_BR");
            faker.RuleFor(x => x.Modalidade, f=> modalidade);
            faker.RuleFor(x => x.AnoLetivo, f => DateTimeExtension.HorarioBrasilia().Year);
            faker.RuleFor(x => x.AnoId, f=> f.PickRandom(1,9));
            ComponenteCurricularEAnoTurmaFiltrosDto = faker.Generate();
        }
    }
}
