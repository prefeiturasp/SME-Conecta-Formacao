using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaCommandHandler : IRequestHandler<SalvarPropostaCommand, bool>
    {
        private readonly IMediator _mediator;

        public SalvarPropostaCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarPropostaCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new SalvarPropostaPublicoAlvoCommand(request.PropostaId, request.Proposta.PublicosAlvo), cancellationToken);

            await _mediator.Send(new SalvarPropostaFuncaoEspecificaCommand(request.PropostaId, request.Proposta.FuncoesEspecificas), cancellationToken);

            await _mediator.Send(new SalvarPropostaCriteriosValidacaoInscricaoCommand(request.PropostaId, request.Proposta.CriteriosValidacaoInscricao), cancellationToken);

            await _mediator.Send(new SalvarPropostaVagaRemanecenteCommand(request.PropostaId, request.Proposta.VagasRemanecentes), cancellationToken);

            await _mediator.Send(new SalvarPropostaPalavraChaveCommand(request.PropostaId, request.Proposta.PalavrasChaves), cancellationToken);

            await _mediator.Send(new SalvarCriterioCertificacaoCommand(request.PropostaId, request.Proposta.CriterioCertificacao), cancellationToken);

            await _mediator.Send(new SalvarPropostaDreCommand(request.PropostaId, request.Proposta.Dres), cancellationToken);

            await _mediator.Send(new SalvarPropostaTurmaCommand(request.PropostaId, request.Proposta.Turmas), cancellationToken);

            await _mediator.Send(new SalvarPropostaTurmaDreCommand(request.Proposta.ObterPropostaTurmasDres), cancellationToken);

            await _mediator.Send(new SalvarPropostaModalidadeCommand(request.PropostaId, request.Proposta.Modalidades), cancellationToken);

            await _mediator.Send(new SalvarPropostaAnoTurmaCommand(request.PropostaId, request.Proposta.AnosTurmas), cancellationToken);

            await _mediator.Send(new SalvarPropostaComponenteCurricularCommand(request.PropostaId, request.Proposta.ComponentesCurriculares), cancellationToken);

            await _mediator.Send(new SalvarPropostaTipoInscricaoCommand(request.PropostaId, request.Proposta.TiposInscricao), cancellationToken);

            if (request.ArquivoImagemDivulgacaoId.GetValueOrDefault() != request.Proposta.ArquivoImagemDivulgacaoId.GetValueOrDefault())
            {
                await _mediator.Send(new ValidarArquivoImagemDivulgacaoPropostaCommand(request.Proposta.ArquivoImagemDivulgacaoId), cancellationToken);

                if (request.ArquivoImagemDivulgacaoId.HasValue)
                    await _mediator.Send(new RemoverArquivoPorIdCommand(request.ArquivoImagemDivulgacaoId.Value), cancellationToken);
            }

            await RemoverCaches(request.PropostaId, cancellationToken);

            return true;
        }

        private async Task RemoverCaches(long propostaId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoResumida.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(propostaId)), cancellationToken);

            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Proposta.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaPublicoAlvo.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaFuncaoEspecifica.Parametros(propostaId)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.DashboardProposta.Parametros(propostaId)), cancellationToken);
        }
    }
}
