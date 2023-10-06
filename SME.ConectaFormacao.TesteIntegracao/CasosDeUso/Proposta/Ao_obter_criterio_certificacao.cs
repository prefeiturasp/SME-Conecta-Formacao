using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_criterio_certificacao : TesteBase
    {
        public Ao_obter_criterio_certificacao(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Proposta - deve obter uma lista de Critérios Certificacao que não foram excluidos")]
        public async Task Deve_obter_lista_criterio_certificacao_nao_excluido()
        {
            // arrange
            var criteriosNaoExcluidos = CriterioCertificacaoMock.GerarCriterioCertificacao(3, false);
            await InserirNaBase(criteriosNaoExcluidos);
            var criteriosExcluidos = CriterioCertificacaoMock.GerarCriterioCertificacao(4, true);
            await InserirNaBase(criteriosExcluidos);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoCriterioCertificacao>();
            
            var retorno = await casoDeUso.Executar();
            
            // act 
            retorno.Any().ShouldBeTrue();
            
            // assert
            retorno.Count().ShouldBeEquivalentTo(3);

        }
    }
}