using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_comunicado_acao_formativa : TesteBase
    {
        public Ao_obter_comunicado_acao_formativa(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter comunicado ação formativa")]
        public async Task Deve_obter_comunicado_acao_formativa()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComunicadoAcaoFormativa>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.ShouldNotBeNull();
            retorno.Descricao.ShouldNotBeEmpty();
            retorno.Url.ShouldNotBeEmpty();
        }
    }
}
