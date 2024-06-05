using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraCommandHandler : IRequestHandler<GerarNotificacaoAreaPromotoraCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoAreaPromotoraCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora,
            IRepositorioUsuario repositorioUsuario)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta);

            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);

                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);

                transacao.Commit();

                foreach (var usuario in notificacao.Usuarios)
                {
                    var destinatario = _mapper.Map<EnviarEmailDto>(usuario);
                    destinatario.Titulo = notificacao.Titulo;
                    destinatario.Texto = notificacao.Mensagem;
                    await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario));
                }
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }

            return true;
        }

        private async Task<Notificacao> ObterNotificacao(Proposta proposta)
        {
            var linkSistema = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacao, DateTimeExtension.HorarioBrasilia().Year));

            var areaPromotora = await _repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);

            var usuarioCriadorProposta = await _repositorioUsuario.ObterPorLogin(proposta.CriadoLogin);

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
