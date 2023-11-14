using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_comunicado_acao_formativa : TestePropostaBase
    {
        public Ao_obter_comunicado_acao_formativa(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - obter comunicado ação formativa")]
        public async Task Deve_obter_comunicado_acao_formativa()
        {
            // arrange 
            var parametroComunicadoAcaoFormativaDescricao = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaDescricao);
            await InserirNaBase(parametroComunicadoAcaoFormativaDescricao);

            var parametroComunicadoAcaoFormativaUrl = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.ComunicadoAcaoFormativaUrl);
            await InserirNaBase(parametroComunicadoAcaoFormativaUrl);
            
            var proposta = await InserirNaBaseProposta();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComunicadoAcaoFormativa>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldNotBeNull();
            retorno.Descricao.ShouldNotBeEmpty();
            retorno.Url.ShouldNotBeEmpty();
        }
    }
}
