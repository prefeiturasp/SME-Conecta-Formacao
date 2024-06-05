using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoDFCommandHandler : IRequestHandler<GerarNotificacaoDFCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoDFCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioUsuario repositorioUsuario)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(GerarNotificacaoDFCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta, request.Parecerista);

            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);

                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);

                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarNotificacao, _mapper.Map<NotificacaoSignalRDTO>(notificacao)));
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

        private async Task<Notificacao> ObterNotificacao(Proposta proposta, PropostaPareceristaResumidoDTO parecerista)
        {
            var linkSistema = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacaoEdicaoProposta, DateTimeExtension.HorarioBrasilia().Year));

            var usuarioResponsavelDF = await _repositorioUsuario.ObterPorLogin(proposta.RfResponsavelDf);

            if (usuarioResponsavelDF.EhNulo())
                throw new Exception(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = _mapper.Map<IEnumerable<NotificacaoUsuario>>(new List<Usuario>() { usuarioResponsavelDF }),

                Titulo = string.Format("Proposta {0} - {1} foi analisada pelo Parecerista",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("O Parecerista {0} ({1}) Inseriu comentários na proposta {2} - {3}. Acesse <a href=\"{4}\">Aqui</a> o cadastro da proposta.",
                    parecerista.Nome,
                    parecerista.Login,
                    proposta.Id,
                    proposta.NomeFormacao,
                    string.Format(linkSistema.Valor,proposta.Id))
            };
        }
    }
}
