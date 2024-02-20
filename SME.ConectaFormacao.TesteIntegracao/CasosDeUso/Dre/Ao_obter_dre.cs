using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre
{
    public class Ao_obter_dres : TesteBase
    {
        public Ao_obter_dres(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Dre - Deve obter as dres com opção de todos")]
        public async Task Deve_obter_dres_com_opcao_todos()
        {
            // arrange 
            var dres = DreMock.GerarDreValida(10, true);
            await InserirNaBase(dres);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaDre>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert
            retorno.Count().ShouldBe(dres.Count());
        }

        [Fact(DisplayName = "Dre - Deve obter as dres sem opção de todos")]
        public async Task Deve_obter_dres_sem_opcao_todos()
        {
            // arrange 
            var dres = DreMock.GerarDreValida(10, false);
            await InserirNaBase(dres);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaDre>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert
            retorno.Count().ShouldBe(dres.Count());
        }
    }
}
