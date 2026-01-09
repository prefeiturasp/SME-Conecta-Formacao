using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Eventos.Codaf
{
    public class NotificarDevolucaoCodafEventHandler(
        IRepositorioCodafMovimentacaoListaPresenca repositorioMovimentacao,
        IRepositorioUsuario repositorioUsuario,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        ITransacao transacao,
        IMediator mediator,
        IMapper mapper) : INotificationHandler<CodafListaPresencaDevolvidaEvento>
    {
        public async Task Handle(CodafListaPresencaDevolvidaEvento notification, CancellationToken cancellationToken)
        {
            var ultimaMovimentacao = await repositorioMovimentacao.ObterUltimaMovimentacaoPorListaPresencaStatusAsync(
                notification.CodafListaPresencaId, StatusCodafListaPresenca.AguardandoDf);

            if (ultimaMovimentacao == null) return;

            var usuarioAlvo = await repositorioUsuario.ObterPorLogin(ultimaMovimentacao.CriadoLogin);
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);

            if (usuarioAlvo == null || usuarioLogado == null) return;

            var notificacao = MontarNotificacao(notification, usuarioAlvo, usuarioLogado);

            using var scope = transacao.Iniciar();
            try
            {
                await repositorioNotificacao.Inserir(notificacao);
                await repositorioNotificacaoUsuario.InserirUsuarios(scope, notificacao.Usuarios, notificacao.Id);
                scope.Commit();
            }
            catch { return; }

            foreach (var usuario in notificacao.Usuarios.Where(u => !string.IsNullOrWhiteSpace(u.Email)))
            {
                var emailDto = mapper.Map<EnviarEmailDto>(usuario);
                emailDto.Titulo = notificacao.Titulo;
                emailDto.Texto = notificacao.Mensagem;
                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, emailDto), cancellationToken);
            }
        }

        private static Notificacao MontarNotificacao(CodafListaPresencaDevolvidaEvento evento, Usuario usuarioAlvo, Usuario usuarioLogado)
        {
            return new Notificacao
            {
                Usuarios = [new NotificacaoUsuario(usuarioAlvo.Login, usuarioAlvo.Nome, usuarioAlvo.Email)],
                CorrelacaoId = evento.Comentario.NotificacaoCorrelacaoId,
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Codaf,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                TipoOrigem = NotificacaoTipoOrigem.DevolucaoParaCorrecaoCodaf,
                Titulo = $"O registro codaf código {evento.CodafListaPresencaId} foi retornado pela DF",
                Mensagem = $"""
                            O admin {usuarioLogado.Nome} ({usuarioLogado.Login}) atuou na Devolução do CODAF à área promotora. 
                            Clique <a href="/formacoes/lista-presenca-codaf/editar/{evento.CodafListaPresencaId}">aqui</a> para acessar.
                            <br /><strong>Comentário:</strong><br />{evento.Comentario.Comentario}
                            """,
                Parametros = new
                {
                    codafListaPresencaId = evento.CodafListaPresencaId,
                    comentarioId = evento.Comentario.Id
                }.ObjetoParaJson()
            };
        }
    }
}