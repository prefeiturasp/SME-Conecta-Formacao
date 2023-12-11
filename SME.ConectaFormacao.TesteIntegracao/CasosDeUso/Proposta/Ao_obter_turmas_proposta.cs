using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_turmas_proposta : TestePropostaBase
    {
        public Ao_obter_turmas_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - deve obter turmas proposta")]
        public async Task Deve_obter_turmas_proposta_valida()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTurmasProposta>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(proposta.QuantidadeTurmas.GetValueOrDefault());
        }

        [Fact(DisplayName = "Proposta - deve retornar excecao ao obter turmas proposta não encontrada")]
        public async Task Deve_retornar_excecao_obter_turmas_proposta_nao_encontrada()
        {
            // arrange
            var id = PropostaMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTurmasProposta>();

            // act 
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(id));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}
