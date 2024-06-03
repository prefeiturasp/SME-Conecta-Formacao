using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_recusar_proposta : TestePropostaBase
    {
        public Ao_recusar_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve recusar proposta com sucesso")]
        public async Task Deve_recusar_proposta_sucesso()
        {
            // arrange
            var perfilLogado = Perfis.ADMIN_DF.ToString();
            CriarClaimUsuario(perfilLogado, "1", "Admin DF");
            await InserirUsuario("1", "Admin DF");

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.AguardandoAnaliseParecerPelaDF);

            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRecusarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaJustificativaDto);

            // assert 
            retorno.ShouldBeTrue();

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(Dominio.Enumerados.SituacaoProposta.Recusada);
        }

        [Fact(DisplayName = "Proposta - Deve retornar excessao proposta não esta na situação aguardando parecer df ao recusar proposta")]
        public async Task Deve_retornar_excessao_proposta_nao_esta_situacao_aguardando_parecer_df()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Cadastrada);

            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRecusarProposta>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaJustificativaDto));

            // assert 
            excessao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECER_DF);
        }

        [Fact(DisplayName = "Proposta - Deve retornar excessao justificativa não informada ao recusar proposta")]
        public async Task Deve_retornar_excessao_justificativa_nao_informada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Cadastrada);

            var propostaJustificativaDto = new Aplicacao.Dtos.Proposta.PropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRecusarProposta>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaJustificativaDto));

            // assert 
            excessao.Mensagens.Contains(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);
        }

        [Fact(DisplayName = "Proposta - Deve retornar excessao proposta não encontrada ao recusar proposta")]
        public async Task Deve_retornar_excessao_proposta_nao_encontrada()
        {
            // arrange
            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRecusarProposta>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(1, propostaJustificativaDto));

            // assert 
            excessao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }
    }
}
