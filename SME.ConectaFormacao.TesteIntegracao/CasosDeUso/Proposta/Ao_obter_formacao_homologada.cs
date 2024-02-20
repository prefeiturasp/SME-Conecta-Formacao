using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_formacao_homologada : TesteBase
    {
        public Ao_obter_formacao_homologada(CollectionFixture collectionFixture, bool limparBanco = false) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Proposta - obter formação homologada")]
        public async Task Deve_obter_formacao_homologada()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterFormacaoHomologada>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)FormacaoHomologada.Sim).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)FormacaoHomologada.NaoCursosPorIN).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)FormacaoHomologada.NaoCursosExtras).ShouldBeTrue();
        }
    }
}
