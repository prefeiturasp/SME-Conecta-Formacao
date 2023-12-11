using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AnoTurma
{
    public class Ao_obter_ano_turma_por_modalidade_ano_letivo : TesteBase
    {
        public Ao_obter_ano_turma_por_modalidade_ano_letivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Ano Turma - Deve obter os anos das turmas por modalidade e ano letivo atual")]
        public async Task Deve_obter_os_anos_das_turmas_por_modalidade_e_ano_letivo()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnosTurmas(9);
            await InserirNaBase(anosTurma);

            AoObterAnosTurmaPorModalidadeAnoLetivoMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAnosPorModalidadeAnoLetivo>();

            // act 
            var retorno = await casoDeUso.Executar(AoObterAnosTurmaPorModalidadeAnoLetivoMock.ModalidadeAnoLetivoFiltrosDTO);

            // assert
            retorno.Count(a=> !a.Todos).ShouldBe(anosTurma.Count(c => c.Modalidade == Modalidade.Fundamental && !c.Todos));
            retorno.Count().ShouldBe(anosTurma.Count(c => c.Modalidade == Modalidade.Fundamental));
        }
        
        [Fact(DisplayName = "Ano Turma - Deve obter todos os anos quando for selecionado todas as modalidades e ano letivo atual")]
        public async Task Deve_obter_todos_quando_selecionado_todas_as_modalidades_e_ano_letivo_atual()
        {
            // arrange 
            var anos = AnoTurmaMock.GerarAnosTurmas(9);
            await InserirNaBase(anos);
            
            AoObterAnosTurmaPorModalidadeAnoLetivoMock.Montar(Modalidade.TODAS);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAnosPorModalidadeAnoLetivo>();

            // act 
            var retorno = await casoDeUso.Executar(AoObterAnosTurmaPorModalidadeAnoLetivoMock.ModalidadeAnoLetivoFiltrosDTO);

            // assert
            retorno.Count().ShouldBe(anos.Count());
        }
    }
}
