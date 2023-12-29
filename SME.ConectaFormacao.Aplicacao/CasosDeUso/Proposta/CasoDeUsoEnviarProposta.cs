using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarProposta : CasoDeUsoAbstrato, ICasoDeUsoEnviarProposta
    {
        public CasoDeUsoEnviarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (proposta.Situacao != SituacaoProposta.Cadastrada)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA);

            var situacao = proposta.FormacaoHomologada == FormacaoHomologada.Sim ?
                SituacaoProposta.AguardandoAnaliseDf :
                SituacaoProposta.Publicada;

            await mediator.Send(new EnviarPropostaCommand(propostaId, situacao));
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));

            if (situacao == SituacaoProposta.Publicada && proposta.FormacaoHomologada != FormacaoHomologada.Sim)
                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarPropostaTurmaVaga, propostaId));

            return true;
        }
    }
}