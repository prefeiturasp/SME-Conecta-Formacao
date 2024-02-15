using MediatR;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalDashboardPorTipoQueryHandler : IRequestHandler<ObterTotalDashboardPorTipoQuery, IEnumerable<QuantidadeTipoDashboardDTO>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterTotalDashboardPorTipoQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<QuantidadeTipoDashboardDTO>> Handle(ObterTotalDashboardPorTipoQuery request, CancellationToken cancellationToken)
        {
            var filtro = request.Filtro;
            return await _repositorioProposta.ObterDashBoardQuantidadePorTipo(filtro.Id, filtro.AreaPromotoraId, filtro.Formato, filtro.PublicoAlvoIds,
                filtro.NomeFormacao, filtro.NumeroHomologacao, filtro.PeriodoRealizacaoInicio, filtro.PeriodoRealizacaoFim,
                filtro.Situacao, filtro.FormacaoHomologada);
        }
    }
}