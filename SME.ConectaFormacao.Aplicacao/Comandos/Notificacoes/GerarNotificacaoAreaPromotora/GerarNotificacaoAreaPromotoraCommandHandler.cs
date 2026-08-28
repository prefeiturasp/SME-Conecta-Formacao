using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoAreaPromotora
{
    public class GerarNotificacaoAreaPromotoraCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora,
        IRepositorioUsuario repositorioUsuario)
        : GerarNotificacaoCommandHandlerBase(transacao, repositorioNotificacao, repositorioNotificacaoUsuario, mediator, mapper),
        IRequestHandler<GerarNotificacaoAreaPromotoraCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta);
            return await ProcessarNotificacaoAsync(notificacao, cancellationToken);
        }

        private async Task<Notificacao> ObterNotificacao(Proposta proposta)
        {
            var linkFormatado = await ObterLinkSistemaPropostaAsync(proposta.Id);

            var areaPromotora = await repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);
            var usuarioCriadorProposta = await repositorioUsuario.ObterPorLogin(proposta.CriadoLogin!);

            var destinatarios = new List<NotificacaoUsuario>()
            {
                new (areaPromotora.Nome,areaPromotora.Email),
                new (usuarioCriadorProposta!.Login, usuarioCriadorProposta.Nome, usuarioCriadorProposta.Email)
            };

            var titulo = string.Format("A Proposta {0} - {1} foi analisada pela Comissão de Análise", proposta.Id, proposta.NomeFormacao);
            var mensagem = string.Format("A proposta {0} - {1} foi analisada pela Comissão de Análise. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e verifique os comentários.",
                    proposta.Id, proposta.NomeFormacao, linkFormatado);

            return CriarNotificacaoAvisoPropostaEmail(proposta, destinatarios, titulo, mensagem);
        }
    }
}
