using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerPareceristaCommandHandler : IRequestHandler<EnviarParecerPareceristaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public EnviarParecerPareceristaCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(EnviarParecerPareceristaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(request.PropostaId), cancellationToken);
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var situacoes = new[] { SituacaoProposta.AguardandoAnalisePeloParecerista, SituacaoProposta.AguardandoReanaliseParecerista };
            if (!situacoes.Contains(proposta.Situacao))
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECERISTA);

            var parecerista = await _mediator.Send(new ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery(request.PropostaId, request.RegistroFuncional), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_LOGADO_NAO_E_PARECERISTA_DA_PROPOSTA);

            parecerista.Situacao = request.Situacao;
            parecerista.Justificativa = request.Justificativa;

            await _repositorioProposta.AtualizarSituacaoParecerista(parecerista.Id, parecerista.RegistroFuncional, parecerista.Situacao, request.Justificativa);

            var pareceristas = await _repositorioProposta.ObterPareceristasPorId(proposta.Id);
            if(proposta.Situacao == SituacaoProposta.AguardandoAnalisePeloParecerista && !pareceristas.Any(a => a.Situacao == SituacaoParecerista.AguardandoValidacao))
            {
                await _mediator.Send(new EnviarPropostaCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerPelaDF), cancellationToken);
                await _mediator.Send(new SalvarPropostaMovimentacaoCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerPelaDF), cancellationToken);
            }
            else if(proposta.Situacao == SituacaoProposta.AguardandoReanaliseParecerista && !pareceristas.Any(a => a.Situacao == SituacaoParecerista.Enviada))
            {
                await _mediator.Send(new EnviarPropostaCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerFinalPelaDF), cancellationToken);
                await _mediator.Send(new SalvarPropostaMovimentacaoCommand(proposta.Id, SituacaoProposta.AguardandoAnaliseParecerFinalPelaDF), cancellationToken);
            }

            return true;
        }
    }
}
