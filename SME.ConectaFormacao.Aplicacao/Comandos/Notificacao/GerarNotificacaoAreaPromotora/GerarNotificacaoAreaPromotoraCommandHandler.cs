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

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora,
        IRepositorioUsuario repositorioUsuario) : IRequestHandler<GerarNotificacaoAreaPromotoraCommand, bool>
    {
        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta);

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

        private async Task<Notificacao> ObterNotificacao(Proposta proposta)
        {
            var linkSistema = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, DateTimeExtension.HorarioBrasilia().Year));

            var areaPromotora = await repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);

            var usuarioCriadorProposta = await repositorioUsuario.ObterPorLogin(proposta.CriadoLogin!);
            
            var destinatarios = new List<NotificacaoUsuario>()
            {
                new (areaPromotora.Nome,areaPromotora.Email),
                new (usuarioCriadorProposta!.Login, usuarioCriadorProposta.Nome, usuarioCriadorProposta.Email)
            };

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,

                Titulo = string.Format("A Proposta {0} - {1} foi analisada pela Comissão de Análise",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("A proposta {0} - {1} foi analisada pela Comissão de Análise. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e verifique os comentários.",
                    proposta.Id,
                    proposta.NomeFormacao,
                    string.Format(linkSistema.Valor,proposta.Id)),

                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = destinatarios
            };
        }
    }
}
