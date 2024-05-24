using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
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

        public ObterNotificacaoQueryHandler(IMapper mapper, IRepositorioNotificacao repositorioNotificacao, IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
        }

        public async Task<NotificacaoDTO> Handle(ObterNotificacaoQuery request, CancellationToken cancellationToken)
        {
            var notificacao = await _repositorioNotificacao.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            if(notificacao.Excluido)
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);

            var notificacaoUsuario = await _repositorioNotificacaoUsuario.ObterNotificacaoUsuario(request.Id, request.Login) ??
                throw new NegocioException(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA_USUARIO);

            if (notificacaoUsuario.Situacao.EhNaoLida())
            {
                notificacaoUsuario.Situacao = NotificacaoUsuarioSituacao.Lida;
                await _repositorioNotificacaoUsuario.Atualizar(notificacaoUsuario);
            }

            return _mapper.Map<NotificacaoDTO>(notificacao);
        }
    }
}
