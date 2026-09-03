using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoParecerista
{
    public class GerarNotificacaoPareceristaCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioUsuario repositorioUsuario)
        : GerarNotificacaoCommandHandlerBase(transacao, repositorioNotificacao, repositorioNotificacaoUsuario, mediator, mapper),
        IRequestHandler<GerarNotificacaoPareceristaCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoPareceristaCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta, request.Pareceristas);
            return await ProcessarNotificacaoAsync(notificacao, cancellationToken);
        }

        private async Task<Notificacao> ObterNotificacao(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            var linkFormatado = await ObterLinkSistemaPropostaAsync(proposta.Id);

            var usuarios = Mapper.Map<IEnumerable<NotificacaoUsuario>>(pareceristas);

            foreach (var usuario in usuarios)
                usuario.Email = (await repositorioUsuario.ObterPorLogin(usuario.Login))!.Email;

            var titulo = string.Format("A Proposta {0} - {1} foi atribuída a você", proposta.Id, proposta.NomeFormacao);
            var mensagem = string.Format("A proposta {0} - {1} foi atribuída a você. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e registre seu parecer.",
                    proposta.Id, proposta.NomeFormacao, linkFormatado);

            return CriarNotificacaoAvisoPropostaEmail(proposta, usuarios, titulo, mensagem);
        }
    }
}
