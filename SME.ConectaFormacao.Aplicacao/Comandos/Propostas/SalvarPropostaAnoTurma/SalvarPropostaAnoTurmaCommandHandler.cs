using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaAnoTurmaCommandHandler : IRequestHandler<SalvarPropostaAnoTurmaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaAnoTurmaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaAnoTurmaCommand request, CancellationToken cancellationToken)
        {
            var anosTurmasAntes = await _repositorioProposta.ObterAnosTurmasPorId(request.PropostaId);

            var anosTurmasInserir = request.AnosTurmas.Where(w => !anosTurmasAntes.Any(a => a.AnoTurmaId == w.AnoTurmaId));
            var anosTurmasExcluir = anosTurmasAntes.Where(w => !request.AnosTurmas.Any(a => a.AnoTurmaId == w.AnoTurmaId));

            if (anosTurmasInserir.Any())
                await _repositorioProposta.InserirAnosTurmas(request.PropostaId, anosTurmasInserir);

            if (anosTurmasExcluir.Any())
                await _repositorioProposta.RemoverAnosTurmas(anosTurmasExcluir);

            return true;
        }
    }
}
