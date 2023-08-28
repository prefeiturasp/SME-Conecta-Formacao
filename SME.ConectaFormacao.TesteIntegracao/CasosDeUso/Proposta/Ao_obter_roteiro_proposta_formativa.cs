using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_roteiro_proposta_formativa : TesteBase
    {
        public Ao_obter_roteiro_proposta_formativa(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - obter ultimo roteiro proposta formativa ativo")]
        public async Task Deve_obter_ultimo_roteiro_proposta_formativa_ativo()
        {
            // arrange 
            var roteiros = RoteiroPropostaFormativaMock.GerarRoteiroPropostaFormativa(3);
            await InserirNaBase(roteiros);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterRoteiroPropostaFormativa>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            var ultimoRoteiroInserido = roteiros.LastOrDefault();

            retorno.Id.ShouldBe(ultimoRoteiroInserido.Id);
            retorno.Descricao.ShouldBe(ultimoRoteiroInserido.Descricao);
        }
    }
}
