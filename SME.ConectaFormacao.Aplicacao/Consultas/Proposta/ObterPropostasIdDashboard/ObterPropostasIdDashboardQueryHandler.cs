using MediatR;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQueryHandler : IRequestHandler<ObterPropostasIdDashboardQuery, long[]>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasIdDashboardQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long[]> Handle(ObterPropostasIdDashboardQuery request, CancellationToken cancellationToken)
        {
            var filtro = request.Filtro;
            var ids= await _repositorioProposta.ObterPropostasIdsDashBoard(filtro.Id,filtro.AreaPromotoraId, filtro.Formato, 
                                                                                 filtro.PublicoAlvoIds, filtro.NomeFormacao, filtro.NumeroHomologacao, 
                                                                                 filtro.PeriodoRealizacaoInicio, filtro.PeriodoRealizacaoFim, filtro.Situacao, filtro.FormacaoHomologada);

            if (ids.PossuiElementos())
                return ids.ToArray();

            return new long[]{};
        }
    }
}