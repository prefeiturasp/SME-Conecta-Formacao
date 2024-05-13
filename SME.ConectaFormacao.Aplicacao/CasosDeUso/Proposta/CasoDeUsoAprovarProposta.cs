using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAprovarProposta : CasoDeUsoAbstrato, ICasoDeUsoAprovarProposta
    {
        public CasoDeUsoAprovarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO)
        { 
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var situacoes = new[] { SituacaoProposta.AguardandoAnaliseParecerPelaDF, SituacaoProposta.AguardandoAnaliseParecerFinalPelaDF };
            if (!situacoes.Contains(proposta.Situacao))
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECER_DF);

            var situacao = SituacaoProposta.Aprovada;

            await mediator.Send(new EnviarPropostaCommand(propostaId, situacao));
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao, propostaJustificativaDTO.Justificativa));

            return true;
        }
    }
}
