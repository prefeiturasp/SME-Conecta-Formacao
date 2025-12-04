using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandler : IRequestHandler<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandHandler(ITransacao transacao, IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario, IMediator mediator, IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora,
            IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao, IRepositorioUsuario repositorioUsuario)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand request, CancellationToken cancellationToken)
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
                    if (usuario.Email.EstaPreenchido())
                    {
                        var destinatario = _mapper.Map<EnviarEmailDto>(usuario);
                        destinatario.Titulo = notificacao.Titulo;
                        destinatario.Texto = notificacao.Mensagem;
                        await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, destinatario));
                    }
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
            var areaPromotora = await _repositorioAreaPromotora.ObterAreaPromotoraPorPropostaId(proposta.Id);

            var propostaMovimentacao = await _repositorioPropostaMovimentacao.ObterPorPropostaId(proposta.Id);

            if (propostaMovimentacao.EhNulo())
                throw new Exception(MensagemNegocio.MOVIMENTACAO_PROPOSTA_NAO_ENCONTRADA);

            var motivo = propostaMovimentacao.Justificativa.EstaPreenchido() ? $"\nMotivo: {propostaMovimentacao.Justificativa}" : string.Empty;

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
                Parametros = new { propostaId = proposta.Id }.ObjetoParaJson(),
                Usuarios = _mapper.Map<IEnumerable<NotificacaoUsuario>>(destinatarios),

                Titulo = string.Format("Parecer final da proposta {0} - {1}",
                    proposta.Id,
                    proposta.NomeFormacao),

                Mensagem = string.Format("Após análise dos pareceristas e da Divisão de formações a proposta {0} - {1} foi {2}.{3}.",
                    proposta.Id,
                    proposta.NomeFormacao,
                    propostaMovimentacao.Situacao.EstaAprovada() ? "aprovada" : "recusada",
                    motivo)
            };
        }
    }
}
