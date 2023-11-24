using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarPropostaParaValidacao : CasoDeUsoAbstrato, ICasoDeUsoEnviarPropostaParaValidacao
    {
        public CasoDeUsoEnviarPropostaParaValidacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (proposta.Situacao != SituacaoProposta.Cadastrada && proposta.Situacao != SituacaoProposta.Devolvida)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA_OU_DEVOLVIDA);

            var situacao = proposta.Situacao == SituacaoProposta.Cadastrada ? SituacaoProposta.AguardandoAnaliseDf : SituacaoProposta.AguardandoAnaliseGestao;

            await mediator.Send(new EnviarPropostaParaAnaliseCommand(propostaId, situacao));

            return await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));
        }
    }
}