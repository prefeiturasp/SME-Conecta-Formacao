using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Notificacao.ObterNotificacao
{
    public class ObterNotificacaoQueryHandler(
        IMapper mapper, 
        IRepositorioNotificacao repositorioNotificacao, 
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, 
        IMediator mediator,
        TimeProvider timeProvider) : IRequestHandler<ObterNotificacaoQuery, NotificacaoDTO>
    {
        public async Task<NotificacaoDTO> Handle(ObterNotificacaoQuery request, CancellationToken cancellationToken)
        {
            var notificacao = await repositorioNotificacao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            if (notificacao.Excluido)
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            var notificacaoUsuario = await repositorioNotificacaoUsuario.ObterNotificacaoUsuario(request.Id, request.Login) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA_USUARIO);

            if (notificacaoUsuario.Situacao.EhNaoLida())
            {
                notificacaoUsuario.Situacao = NotificacaoUsuarioSituacao.Lida;
                await repositorioNotificacaoUsuario.Atualizar(notificacaoUsuario);

                notificacao.Usuarios = [notificacaoUsuario];
                await EnviarNotificacaoLidaSignalR(notificacao, cancellationToken);
            }

            var notificacaoDto = mapper.Map<NotificacaoDTO>(notificacao);
            if (notificacao.DataExpiracao.HasValue && notificacao.DataExpiracao.Value < timeProvider.GetUtcNow())
            {
                notificacaoDto.Mensagem = notificacao.MensagemAposExpiracao ?? "Mensagem expirada";
            }

            return notificacaoDto;
        }

        private async Task EnviarNotificacaoLidaSignalR(Dominio.Entidades.Notificacao notificacao, CancellationToken cancellationToken)
        {
            var notificacaoSignalR = mapper.Map<NotificacaoSignalRDTO>(notificacao);
            await mediator.Send(new EnviarNotificacaoLidaCommand(notificacaoSignalR), cancellationToken);
        }
    }
}
