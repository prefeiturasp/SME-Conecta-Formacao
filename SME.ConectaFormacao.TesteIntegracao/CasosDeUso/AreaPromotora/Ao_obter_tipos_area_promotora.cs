using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora
{
    public class Ao_obter_tipos_area_promotora : TesteBase
    {
        public Ao_obter_tipos_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Área Promotora - Deve retornar os tipos de área conhecimento")]
        public async Task Deve_retornar_lista_de_tipos_de_area_conhecimento()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTiposAreaPromotora>();

            // act
            var retorno = await casoDeUso.Executar();

            // assert
            retorno.Any().ShouldBeTrue();
        }
    }
}
