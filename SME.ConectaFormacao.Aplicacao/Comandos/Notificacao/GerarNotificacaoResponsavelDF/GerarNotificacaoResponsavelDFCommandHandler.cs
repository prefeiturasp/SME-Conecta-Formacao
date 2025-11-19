using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
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
    public class GerarNotificacaoResponsavelDFCommandHandler : IRequestHandler<GerarNotificacaoResponsavelDFCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoResponsavelDFCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioUsuario repositorioUsuario,
            IRepositorioProposta repositorioProposta)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(GerarNotificacaoResponsavelDFCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacao(request.Proposta, request.Parecerista);

            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);

                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);

                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarNotificacao, notificacao));
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

            var motivo = await _repositorioProposta.ObterPareceristasPorPropostaId(proposta.Id);

            var situacao = motivo.FirstOrDefault(f => f.RegistroFuncional.Equals(parecerista.Login)).Situacao.EstaAprovada() ? "aprovação" : "recusa";

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = _mapper.Map<IEnumerable<NotificacaoUsuario>>(new List<Usuario>() { usuarioResponsavelDF }),

                Titulo = string.Format("A Proposta {0} - {1} foi analisada pelo Parecerista",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("O Parecerista  {0} - ({1}) sugeriu a {2} da proposta {3} - {4}. Motivo: {5} \nAcesse <a href=\"{6}\">Aqui</a> o cadastro da proposta.",
                    parecerista.Nome,
                    parecerista.Login,
                    situacao,
                    proposta.Id,
                    proposta.NomeFormacao,
                    motivo.FirstOrDefault(f => f.RegistroFuncional.Equals(parecerista.Login)).Justificativa,
                    string.Format(linkSistema.Valor,proposta.Id))
            };
        }
    }
}
