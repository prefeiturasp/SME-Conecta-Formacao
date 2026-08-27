using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandler(
        IServicoNotificacao servicoNotificacao,
        IMapper mapper,
        IRepositorioAreaPromotora repositorioAreaPromotora,
        IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao,
        IRepositorioUsuario repositorioUsuario) : IRequestHandler<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta);

            return await servicoNotificacao.PersistirEEnviarAsync(notificacao, cancellationToken);
        }

        private async Task<Notificacao> ObterNotificacao(Proposta proposta)
        {
            var areaPromotora = await repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);

            var propostaMovimentacao = await repositorioPropostaMovimentacao.ObterPorPropostaId(proposta.Id);

            if (propostaMovimentacao.EhNulo())
                throw new Exception(MensagemNegocio.MOVIMENTACAO_PROPOSTA_NAO_ENCONTRADA);

            var motivo = propostaMovimentacao.Justificativa.EstaPreenchido() ? $"\nMotivo: {propostaMovimentacao.Justificativa}" : string.Empty;

            var usuarioCriadorProposta = await repositorioUsuario.ObterPorLogin(proposta.CriadoLogin);

            var destinatarios = new List<NotificacaoUsuario>()
            {
                new (areaPromotora.Nome,areaPromotora.Email),
                new (usuarioCriadorProposta.Login, usuarioCriadorProposta.Nome, usuarioCriadorProposta.Email)
            };

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = mapper.Map<IEnumerable<NotificacaoUsuario>>(destinatarios),

                Titulo = string.Format("Parecer final da proposta {0} - {1}",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("Após análise dos pareceristas e da Divisão de formações a proposta {0} - {1} foi {2}.{3}.",
                    proposta.Id,
                    proposta.NomeFormacao,
                    propostaMovimentacao.Situacao.EstaAprovada() ? "aprovada" : "recusada",
                    motivo)
            };
        }
    }
}
