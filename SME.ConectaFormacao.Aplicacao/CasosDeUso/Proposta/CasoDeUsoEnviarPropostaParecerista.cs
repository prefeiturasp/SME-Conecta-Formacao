using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarPropostaParecerista : CasoDeUsoAbstrato, ICasoDeUsoEnviarPropostaParecerista
    {
        public CasoDeUsoEnviarPropostaParecerista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId)
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new EnviarParecerPareceristaCommand(propostaId, usuarioLogado.Login, SituacaoParecerista.Enviada, string.Empty));
        }
    }
}
