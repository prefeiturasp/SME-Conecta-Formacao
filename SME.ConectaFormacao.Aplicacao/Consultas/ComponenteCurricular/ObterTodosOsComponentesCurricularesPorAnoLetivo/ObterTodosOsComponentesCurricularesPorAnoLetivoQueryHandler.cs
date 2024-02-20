using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsComponentesCurricularesPorAnoLetivoQueryHandler : IRequestHandler<ObterTodosOsComponentesCurricularesPorAnoLetivoQuery, IEnumerable<ComponenteCurricular>>
    {
        private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;

        public ObterTodosOsComponentesCurricularesPorAnoLetivoQueryHandler(IRepositorioComponenteCurricular repositorioComponenteCurricular)
        {
            _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        }

        public async Task<IEnumerable<ComponenteCurricular>> Handle(ObterTodosOsComponentesCurricularesPorAnoLetivoQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioComponenteCurricular.ObterPorAnoLetivo(request.AnoLetivo);
        }
    }
}
