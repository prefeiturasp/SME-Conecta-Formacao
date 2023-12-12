using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_encontro_proposta_paginada : TestePropostaBase
    {
        public Ao_obter_encontro_proposta_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter os encontros paginados ")]
        public async Task Deve_obter_encontro_paginado()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaEncontroPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalRegistros.ShouldBe(proposta.Encontros.Count());
        }
    }
}
