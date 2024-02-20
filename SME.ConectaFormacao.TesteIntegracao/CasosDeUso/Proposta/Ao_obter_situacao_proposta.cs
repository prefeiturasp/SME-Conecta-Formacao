using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_situacao_proposta : TesteBase
    {
        public Ao_obter_situacao_proposta(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter situações de proposta")]
        public async Task Deve_obter_tipos_formacao()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterSituacoesProposta>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)SituacaoProposta.Cadastrada).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)SituacaoProposta.Rascunho).ShouldBeTrue();
        }
    }
}
