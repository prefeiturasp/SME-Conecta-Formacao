using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        IRepositorioCodafComentarioListaPresenca repositorioCodafComentario,
        ITransacao transacao,
        IMediator mediator,
        IMapper mapper,
        ILogger<NotificarDevolucaoCodafEventHandler> logger) : INotificationHandler<CodafListaPresencaDevolvidaEvento>
    {
        public async Task Handle(CodafListaPresencaDevolvidaEvento eventoNotificacao, CancellationToken cancellationToken)
        {
            try
            {
                var ultimaMovimentacao = await repositorioMovimentacao.ObterUltimaMovimentacaoPorListaPresencaStatusAsync(
                eventoNotificacao.CodafListaPresenca.Id, StatusCodafListaPresenca.AguardandoDf);

                if (ultimaMovimentacao == null) return;

                var usuarioAlvo = await repositorioUsuario.ObterPorLogin(ultimaMovimentacao.CriadoLogin);
                var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);

                if (usuarioAlvo == null || usuarioLogado == null) return;

                var notificacao = MontarNotificacao(eventoNotificacao, usuarioAlvo, usuarioLogado);

                using var scope = transacao.Iniciar();
                    await repositorioNotificacao.Inserir(notificacao);
                    await repositorioNotificacaoUsuario.InserirUsuarios(scope, notificacao.Usuarios, notificacao.Id);
                    eventoNotificacao.Comentario.MarcarNotificacaoEnviada();
                    await repositorioCodafComentario.Atualizar(eventoNotificacao.Comentario);
                    scope.Commit();

                foreach (var usuario in notificacao.Usuarios.Where(u => !string.IsNullOrWhiteSpace(u.Email)))
                {
                    var emailDto = mapper.Map<EnviarEmailDto>(usuario);
                    emailDto.Titulo = notificacao.Titulo;
                    emailDto.Texto = notificacao.Mensagem;
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, emailDto), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao notificar devolução do CODAF {Id}", eventoNotificacao.CodafListaPresenca.Id);
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
                Titulo = $"O CODAF para a formação {evento.CodafListaPresenca.Proposta.NumeroHomologacao} - {evento.CodafListaPresenca.Proposta.NomeFormacao}, turma {evento.CodafListaPresenca.PropostaTurma.Nome} foi devolvida pela DF pelo usuário {usuarioLogado.Nome}",
                Mensagem = $"""
                            O admin {usuarioLogado.Nome} ({usuarioLogado.Login}) atuou na Devolução do CODAF à área promotora. 
                            Clique <a href="/formacoes/lista-presenca-codaf/editar/{evento.CodafListaPresenca.Id}">aqui</a> para acessar.
                            <br /><br /><strong>Comentário:</strong><br />{evento.Comentario.Comentario}
                            """,
                Parametros = new
                {
                    codafListaPresencaId = evento.CodafListaPresenca.Id,
                    comentarioId = evento.Comentario.Id
                }.ObjetoParaJson()
            };
        }
    }
}