using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks
{
    public static class AoObterAnosTurmaPorModalidadeAnoLetivoMock
    {
        public static ModalidadeAnoLetivoFiltrosDTO ModalidadeAnoLetivoFiltrosDTO { get; set; }

        public static void Montar(Modalidade modalidade, bool todos = false)
        {
            var faker = new Faker<ModalidadeAnoLetivoFiltrosDTO>("pt_BR");
            faker.RuleFor(x => x.Modalidade, f=> modalidade);
            faker.RuleFor(x => x.AnoLetivo, f => DateTimeExtension.HorarioBrasilia().Year);
            ModalidadeAnoLetivoFiltrosDTO = faker.Generate();
        }
    }
}
