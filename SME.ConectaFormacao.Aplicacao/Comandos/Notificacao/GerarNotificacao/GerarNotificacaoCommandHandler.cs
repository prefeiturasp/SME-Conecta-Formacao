using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoCommandHandler : IRequestHandler<GerarNotificacaoCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;

        public GerarNotificacaoCommandHandler(IRepositorioProposta repositorioProposta, ITransacao transacao,IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await ObterNotificacaoPorProposta(request.Proposta);
            
            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);
                
                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);
                
                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.NotificarPropostaUsuario, notificacao));
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

        private async Task<Notificacao> ObterNotificacaoPorProposta(Proposta proposta)
        {
            return proposta.Situacao switch
            {
                SituacaoProposta.AguardandoAnaliseDf => await ObterNotificacaoParaDF(proposta),
                SituacaoProposta.AguardandoAnalisePeloParecerista => await ObterNotificacaoPareceristas(proposta),
                SituacaoProposta.AguardandoAnaliseParecerPelaDF => await ObterNotificacaoParaDF(proposta),
                SituacaoProposta.AnaliseParecerPelaAreaPromotora => await ObterNotificacaoParaAreaPromotora(proposta),
                SituacaoProposta.AguardandoReanalisePeloParecerista => await ObterNotificacaoPareceristas(proposta),
                SituacaoProposta.AguardandoValidacaoFinalPelaDF => await ObterNotificacaoParaDF(proposta),
                _ => throw new ArgumentOutOfRangeException("Essa situação de proposta não tem suporte a notificações")
            };
        }

        private async Task<Notificacao> ObterNotificacaoPareceristas(Proposta proposta)
        {
            var pareceristas = await _repositorioProposta.ObterPareceristasPorId(proposta.Id);

            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Titulo = string.Format("Proposta '{0}' - '{1}'", proposta.Id, proposta.NomeFormacao),
                Mensagem = string.Format("A proposta '{0}' - '{1}' foi atribuída a você. Acesse aqui o cadastro da proposta e registre seu parecer."),
                Parametros = JsonConvert.SerializeObject(proposta),
                Usuarios =  pareceristas.Select(s=> new NotificacaoUsuario() { RegistroFuncional = s.RegistroFuncional})
            };
        }

        private async Task<Notificacao> ObterNotificacaoParaAreaPromotora(Proposta proposta)
        {
            throw new NotImplementedException();
        }
        
        private async Task<Notificacao> ObterNotificacaoParaDF(Proposta proposta)
        {
            throw new NotImplementedException();
        }
    }
}
