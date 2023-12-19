using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsAnosTurmaQueryHandler : IRequestHandler<ObterTodosOsAnosTurmaPorAnoLetivoQuery, IEnumerable<AnoTurma>>
    {
        private readonly IRepositorioAnoTurma _repositorioAnoTurma;

        public ObterTodosOsAnosTurmaQueryHandler(IRepositorioAnoTurma repositorioAnoTurma)
        {
            _repositorioAnoTurma = repositorioAnoTurma ?? throw new ArgumentNullException(nameof(repositorioAnoTurma));
        }

        public async Task<IEnumerable<AnoTurma>> Handle(ObterTodosOsAnosTurmaPorAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioAnoTurma.ObterPorAnoLetivo(request.AnoLetivo);
        }
    }
}
