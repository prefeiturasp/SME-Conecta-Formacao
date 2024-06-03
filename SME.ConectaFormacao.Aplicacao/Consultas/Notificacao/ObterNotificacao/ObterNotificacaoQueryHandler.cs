using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Notificacao.ObterNotificacao
{
    public class ObterNotificacaoQueryHandler : IRequestHandler<ObterNotificacaoQuery, NotificacaoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IMediator _mediator;

        public ObterNotificacaoQueryHandler(IMapper mapper, IRepositorioNotificacao repositorioNotificacao, IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<NotificacaoDTO> Handle(ObterNotificacaoQuery request, CancellationToken cancellationToken)
        {
            var notificacao = await _repositorioNotificacao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            if (notificacao.Excluido)
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            var notificacaoUsuario = await _repositorioNotificacaoUsuario.ObterNotificacaoUsuario(request.Id, request.Login) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA_USUARIO);

            if (notificacaoUsuario.Situacao.EhNaoLida())
            {
                notificacaoUsuario.Situacao = NotificacaoUsuarioSituacao.Lida;
                await _repositorioNotificacaoUsuario.Atualizar(notificacaoUsuario);

                notificacao.Usuarios = new List<NotificacaoUsuario>() { notificacaoUsuario };
                await EnviarNotificacaoLidaSignalR(notificacao, cancellationToken);
            }

            return _mapper.Map<NotificacaoDTO>(notificacao);
        }

        private async Task EnviarNotificacaoLidaSignalR(Dominio.Entidades.Notificacao notificacao, CancellationToken cancellationToken)
        {
            var notificacaoSignalR = _mapper.Map<NotificacaoSignalRDTO>(notificacao);
            await _mediator.Send(new EnviarNotificacaoLidaCommand(notificacaoSignalR), cancellationToken);
        }
    }
}
