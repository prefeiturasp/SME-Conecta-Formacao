using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoReanaliseParecerista
{
    public class GerarNotificacaoReanalisePareceristaCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioUsuario repositorioUsuario)
        : GerarNotificacaoCommandHandlerBase(transacao, repositorioNotificacao, repositorioNotificacaoUsuario, mediator, mapper),
        IRequestHandler<GerarNotificacaoReanalisePareceristaCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoReanalisePareceristaCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta, request.Pareceristas);
            return await ProcessarNotificacaoAsync(notificacao, cancellationToken);
        }

        private async Task<Notificacao> ObterNotificacao(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            var linkFormatado = await ObterLinkSistemaPropostaAsync(proposta.Id);

            var usuarios = Mapper.Map<IEnumerable<NotificacaoUsuario>>(pareceristas);

            foreach (var usuario in usuarios)
            {
                var usuarioDb = await repositorioUsuario.ObterPorLogin(usuario.Login);
                if (usuarioDb is not null)
                    usuario.Email = usuarioDb.Email;
            }

            var titulo = string.Format("Proposta {0} - {1} foi atribuída a você", proposta.Id, proposta.NomeFormacao);
            var mensagem = string.Format("A proposta {0} - {1} foi atribuída a você. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e registre seu parecer final.",
                    proposta.Id, proposta.NomeFormacao, linkFormatado);

            return CriarNotificacaoAvisoPropostaEmail(proposta, usuarios, titulo, mensagem);
        }
    }
}
