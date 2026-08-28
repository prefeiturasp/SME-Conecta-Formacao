using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Extensoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoReanalisePareceristaCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioUsuario repositorioUsuario) : IRequestHandler<GerarNotificacaoReanalisePareceristaCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoReanalisePareceristaCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta, request.Pareceristas);

            var transacaoDb = transacao.Iniciar();
            try
            {
                var notificacaoId = await repositorioNotificacao.Inserir(notificacao);

                await repositorioNotificacaoUsuario.InserirUsuarios(transacaoDb, notificacao.Usuarios, notificacaoId);

                transacaoDb.Commit();

                var usuariosUnicos = notificacao.Usuarios.RemoverDuplicatasPorEmail();

                foreach (var usuario in usuariosUnicos)
                {
                    var destinatario = mapper.Map<EnviarEmailDto>(usuario);
                    destinatario.Titulo = notificacao.Titulo;
                    destinatario.Texto = notificacao.Mensagem;
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario), cancellationToken);
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

        private async Task<Notificacao> ObterNotificacao(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            var linkSistema = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, DateTimeExtension.HorarioBrasilia().Year));

            var usuarios = mapper.Map<IEnumerable<NotificacaoUsuario>>(pareceristas);

            foreach (var usuario in usuarios)
            {
                var usuarioDb = await repositorioUsuario.ObterPorLogin(usuario.Login);
                if (usuarioDb is not null)
                    usuario.Email = usuarioDb.Email;
            }
                

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = usuarios,

                Titulo = string.Format("Proposta {0} - {1} foi atribuída a você",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("A proposta {0} - {1} foi atribuída a você. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e registre seu parecer final.",
                    proposta.Id,
                    proposta.NomeFormacao,
                    string.Format(linkSistema.Valor,proposta.Id))
            };
        }
    }
}
