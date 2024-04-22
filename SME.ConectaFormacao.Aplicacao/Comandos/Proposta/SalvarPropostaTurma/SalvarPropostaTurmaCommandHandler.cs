using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaCommandHandler : IRequestHandler<SalvarPropostaTurmaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public SalvarPropostaTurmaCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarPropostaTurmaCommand request, CancellationToken cancellationToken)
        {
            var turmasAntes = await _repositorioProposta.ObterTurmasPorId(request.PropostaId);

            var turmasInserir = request.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
            var turmasAlterar = turmasAntes.Where(w => request.Turmas.Any(a => a.Id == w.Id && a.Nome != w.Nome)).ToList();
            var turmasExcluir = turmasAntes.Where(w => !request.Turmas.Any(a => a.Id == w.Id));

            if (turmasInserir.Any())
            {
                await _repositorioProposta.InserirTurmas(request.PropostaId, turmasInserir);

                if (request.Situacao.EhAlterando())
                    await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarPropostaTurmaVaga, request.PropostaId), cancellationToken);
            }

            if (turmasAlterar.Any())
            {
                foreach (var turmaAlterar in turmasAlterar)
                {
                    var turma = request.Turmas.FirstOrDefault(t => t.Id == turmaAlterar.Id);
                    turmaAlterar.Nome = turma.Nome;
                }

                await _repositorioProposta.AtualizarTurmas(request.PropostaId, turmasAlterar);
            }

            if (turmasExcluir.Any())
                await _repositorioProposta.RemoverTurmas(turmasExcluir);

            return true;
        }
    }
}
