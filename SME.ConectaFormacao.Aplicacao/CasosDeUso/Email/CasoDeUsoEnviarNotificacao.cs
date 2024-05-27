using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Email
{
    public class CasoDeUsoEnviarNotificacao : CasoDeUsoAbstrato, ICasoDeUsoEnviarNotificacao
    {
        public CasoDeUsoEnviarNotificacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var enviarNotificacao = param.ObterObjetoMensagem<NotificacaoSignalRDTO>() ?? throw new NegocioException(MensagemNegocio.DADOS_ENVIO_NOTIFICACAO_NAO_LOCALIZADO);

            await mediator.Send(new EnviarNotificacaoCommand(enviarNotificacao));

            return true;
        }
    }
}