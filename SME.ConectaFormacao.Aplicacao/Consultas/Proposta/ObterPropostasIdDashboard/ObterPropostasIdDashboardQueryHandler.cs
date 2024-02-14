using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQueryHandler : IRequestHandler<ObterPropostasIdDashboardQuery, IEnumerable<Proposta>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasIdDashboardQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<Proposta>> Handle(ObterPropostasIdDashboardQuery request, CancellationToken cancellationToken)
        {
            var filtro = request.Filtro;
            var propostas= await _repositorioProposta.ObterPropostasIdsDashBoard(filtro.Id,filtro.AreaPromotoraId, filtro.Formato, 
                                                                                 filtro.PublicoAlvoIds, filtro.NomeFormacao, filtro.NumeroHomologacao, 
                                                                                 filtro.PeriodoRealizacaoInicio, filtro.PeriodoRealizacaoFim, filtro.Situacao, filtro.FormacaoHomologada,request.Situacao);

            if (propostas.PossuiElementos())
                return propostas;

            return new List<Proposta>(){};
        }
    }
}