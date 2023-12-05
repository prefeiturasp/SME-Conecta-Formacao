using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Ano
{
    public class Ao_obter_ano_por_modalidade_ano_letivo : TesteBase
    {
        public Ao_obter_ano_por_modalidade_ano_letivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Ano - Deve obter os anos por modalidade e ano letivo atual")]
        public async Task Deve_obter_os_anos_por_modalidade_e_ano_letivo()
        {
            // arrange 
            var anos = AnoMock.GerarAno(9);
            await InserirNaBase(anos);

            var anosPErsistidos = ObterTodos<Dominio.Entidades.Ano>();
            AoObterAnosPorModalidadeAnoLetivoMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAnosPorModalidadeAnoLetivo>();

            // act 
            var retorno = await casoDeUso.Executar(AoObterAnosPorModalidadeAnoLetivoMock.ModalidadeAnoLetivoFiltrosDTO);

            // assert
            retorno.Count(a=> !a.Todos).ShouldBe(anos.Count(c => c.Modalidade == Modalidade.Fundamental && !c.Todos));
            retorno.Count().ShouldBe(anos.Count(c => c.Modalidade == Modalidade.Fundamental));
        }
        
        [Fact(DisplayName = "Ano - Deve obter todos os anos quando for selecionado todas as modalidades e ano letivo atual")]
        public async Task Deve_obter_todos_quando_selecionado_todas_as_modalidades_e_ano_letivo_atual()
        {
            // arrange 
            var anos = AnoMock.GerarAno(9);
            await InserirNaBase(anos);
            AoObterAnosPorModalidadeAnoLetivoMock.Montar(Modalidade.TODAS);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAnosPorModalidadeAnoLetivo>();

            // act 
            var retorno = await casoDeUso.Executar(AoObterAnosPorModalidadeAnoLetivoMock.ModalidadeAnoLetivoFiltrosDTO);

            // assert
            retorno.Count().ShouldBe(anos.Count());
        }
    }
}
