using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Extensoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes
{
    public abstract class GerarNotificacaoCommandHandlerBase(
        ITransacao transacao,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        IMediator mediator,
        IMapper mapper)
    {
        protected readonly ITransacao Transacao = transacao;
        protected readonly IRepositorioNotificacao RepositorioNotificacao = repositorioNotificacao;
        protected readonly IRepositorioNotificacaoUsuario RepositorioNotificacaoUsuario = repositorioNotificacaoUsuario;
        protected readonly IMediator Mediator = mediator;
        protected readonly IMapper Mapper = mapper;

        protected async Task<bool> ProcessarNotificacaoAsync(Notificacao notificacao, CancellationToken cancellationToken)
        {
            var transacaoDb = Transacao.Iniciar();
            try
            {
                var notificacaoId = await RepositorioNotificacao.Inserir(notificacao);

                await RepositorioNotificacaoUsuario.InserirUsuarios(transacaoDb, notificacao.Usuarios, notificacaoId);

                transacaoDb.Commit();

                var usuariosUnicos = notificacao.Usuarios.RemoverDuplicatasPorEmail();

                foreach (var usuario in usuariosUnicos)
                {
                    var destinatario = Mapper.Map<EnviarEmailDto>(usuario);
                    destinatario.Titulo = notificacao.Titulo;
                    destinatario.Texto = notificacao.Mensagem;
                    await Mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);
                }
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

            return true;
        }

        protected async Task<string> ObterLinkSistemaPropostaAsync(long propostaId)
        {
            var linkSistema = await Mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, DateTimeExtension.HorarioBrasilia().Year));
            return string.Format(linkSistema.Valor, propostaId);
        }

        protected static Notificacao CriarNotificacaoAvisoPropostaEmail(Proposta proposta, IEnumerable<NotificacaoUsuario> usuarios, string titulo, string mensagem)
        {
            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = usuarios,
                Titulo = titulo,
                Mensagem = mensagem
            };
        }
    }
}
