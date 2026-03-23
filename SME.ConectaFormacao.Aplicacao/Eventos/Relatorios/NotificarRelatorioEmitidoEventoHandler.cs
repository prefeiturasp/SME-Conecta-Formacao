using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Eventos.Relatorios
{
    public class NotificarRelatorioEmitidoEventoHandler(
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        ITransacao transacao,
        IMapper mapper,
        IMediator mediator,
        IServicoLogs servicoLogs) : INotificationHandler<NotificarRelatorioEmitidoEvento>
    {
        public async Task Handle(NotificarRelatorioEmitidoEvento notification, CancellationToken cancellationToken)
        {
            try
            {
                var notificacao = MontarNotificacao(notification.Notificacao, notification.UsuariosAlvo);

                using var scope = transacao.Iniciar();
                await repositorioNotificacao.Inserir(notificacao);
                await repositorioNotificacaoUsuario.InserirUsuarios(scope, notificacao.Usuarios, notificacao.Id);
                scope.Commit();

                await mediator.Send(
                    new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarNotificacao, mapper.Map<NotificacaoSignalRDTO>(notificacao)),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await servicoLogs.Enviar(
                    $"Erro a notificar relatório emitido: {ex.Message}",
                    LogContexto.Notificacao, LogNivel.Critico,
                    $"Titulo={notification.Notificacao.Titulo} | Id={notification.Notificacao.Id}",
                    ex.StackTrace);
            }
        }
        private static Notificacao MontarNotificacao(NotificacaoDTO dto, List<Usuario> usuariosAlvo)
        {
            return new Notificacao
            {
                Titulo = dto.Titulo,
                Mensagem = dto.Mensagem,
                Categoria = dto.Categoria,
                Tipo = dto.Tipo,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                TipoOrigem = NotificacaoTipoOrigem.Relatorio,
                Parametros = dto.Parametros,
                DataExpiracao = dto.DataExpiracao,
                MensagemAposExpiracao = dto.MensagemAposExpiracao,
                Usuarios = [.. usuariosAlvo.Select(u => new NotificacaoUsuario(u.Login, u.Nome, u.Email))]
            };
        }
    }
}