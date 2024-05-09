using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_aprovar_proposta : TestePropostaBase
    {
        public Ao_aprovar_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName ="Proposta - Deve aprovar proposta com sucesso com justificativa")]
        public async Task Deve_aprovar_proposta_sucesso_com_justificativa()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.AguardandoAnaliseParecerPelaDF);

            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaJustificativaDto);

            // assert 
            retorno.ShouldBeTrue();

            var propostaBanco =  ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(Dominio.Enumerados.SituacaoProposta.Aprovada);
        }

        [Fact(DisplayName = "Proposta - Deve aprovar proposta com sucesso sem justificativa")]
        public async Task Deve_aprovar_proposta_sucesso_sem_justificativa()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.AguardandoAnaliseParecerPelaDF);

            var propostaJustificativaDto = new Aplicacao.Dtos.Proposta.PropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaJustificativaDto);

            // assert 
            retorno.ShouldBeTrue();

            var propostaBanco = ObterPorId<Dominio.Entidades.Proposta, long>(proposta.Id);
            propostaBanco.Situacao.ShouldBe(Dominio.Enumerados.SituacaoProposta.Aprovada);
        }

        [Fact(DisplayName = "Proposta - Deve retornar excessao proposta não esta na situação aguardando parecer df ao aprovar proposta")]
        public async Task Deve_retornar_excessao_proposta_nao_esta_situacao_aguardando_parecer_df()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Cadastrada);

            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarProposta>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaJustificativaDto));

            // assert 
            excessao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECER_DF);
        }

        [Fact(DisplayName = "Proposta - Deve retornar excessao proposta não encontrada ao aprovar proposta")]
        public async Task Deve_retornar_excessao_proposta_nao_encontrada()
        {
            // arrange
            var propostaJustificativaDto = PropostaJustificativaMock.GerarPropostaJustificativaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAprovarProposta>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(1, propostaJustificativaDto));

            // assert 
            excessao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }
    }
}
