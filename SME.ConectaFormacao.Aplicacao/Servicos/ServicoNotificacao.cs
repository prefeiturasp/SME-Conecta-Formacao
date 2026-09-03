using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Extensoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Servicos
{
    public class ServicoNotificacao(
        ITransacao transacao,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        IMediator mediator,
        IMapper mapper) : IServicoNotificacao
    {
        public async Task<bool> PersistirEEnviarAsync(Notificacao notificacao, CancellationToken cancellationToken = default)
        {
            var transacaoDb = transacao.Iniciar();
            try
            {
                var notificacaoId = await repositorioNotificacao.Inserir(notificacao);
                await repositorioNotificacaoUsuario.InserirUsuarios(transacaoDb, notificacao.Usuarios, notificacaoId);

                transacaoDb.Commit();

                await EnviarEmailsAsync(notificacao, cancellationToken);

                return true;
            }
            catch
            {
                transacaoDb.Rollback();
                throw;
            }
            finally
            {
                transacaoDb.Dispose();
            }
        }

        private async Task EnviarEmailsAsync(Notificacao notificacao, CancellationToken cancellationToken)
        {
            var usuariosUnicos = notificacao.Usuarios.RemoverDuplicatasPorEmail();

            foreach (var usuario in usuariosUnicos)
            {
                var destinatario = mapper.Map<EnviarEmailDto>(usuario);
                destinatario.Titulo = notificacao.Titulo;
                destinatario.Texto = notificacao.Mensagem;

                await mediator.Send(
                    new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario),
                    cancellationToken);
            }
        }
    }
}
