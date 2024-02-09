using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasDashboardQueryHandler : IRequestHandler<ObterPropostasDashboardQuery,IEnumerable<Proposta>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasDashboardQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<Proposta>> Handle(ObterPropostasDashboardQuery request, CancellationToken cancellationToken)
        {
            var filtro = request.Filtro;
            return await _repositorioProposta.ObterPropostasDashBoardPorTipo(filtro.Id,filtro.AreaPromotoraId,filtro.Formato,
                                                                             filtro.PublicoAlvoIds,filtro.NomeFormacao,filtro.NumeroHomologacao,
                                                                             filtro.PeriodoRealizacaoInicio,filtro.PeriodoRealizacaoFim,
                                                                             filtro.Situacao,filtro.FormacaoHomologada);
        }
    }
}