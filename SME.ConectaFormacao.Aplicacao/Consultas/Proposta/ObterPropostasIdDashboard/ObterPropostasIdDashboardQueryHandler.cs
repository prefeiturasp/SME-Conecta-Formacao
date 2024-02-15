using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQueryHandler : IRequestHandler<ObterPropostasIdDashboardQuery, IEnumerable<long>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasIdDashboardQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public Task<IEnumerable<long>> Handle(ObterPropostasIdDashboardQuery request, CancellationToken cancellationToken)
        {
            return _repositorioProposta.ObterPropostasIdsDashBoard(
                request.AreaPromotoraIdUsuarioLogado,
                request.Filtro.Id,
                request.Filtro.AreaPromotoraId, 
                request.Filtro.Formato,
                request.Filtro.PublicoAlvoIds,
                request.Filtro.NomeFormacao, 
                request.Filtro.NumeroHomologacao,
                request.Filtro.PeriodoRealizacaoInicio, 
                request.Filtro.PeriodoRealizacaoFim, 
                request.Filtro.Situacao, 
                request.Filtro.FormacaoHomologada, 
                request.Situacao);
        }
    }
}