using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdsPorFiltroQueryHandler : IRequestHandler<ObterPropostasIdsPorFiltroQuery, IEnumerable<long>>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        
        public ObterPropostasIdsPorFiltroQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }
        
        public Task<IEnumerable<long>> Handle(ObterPropostasIdsPorFiltroQuery request, CancellationToken cancellationToken)
        {
            var filtro = request.FiltroListagemFormacaoDTO;
            
            return _repositorioProposta.ObterListagemFormacoesPorFiltro(filtro.PublicosAlvosIds, 
                filtro.Titulo, filtro.AreasPromotorasIds, filtro.DataFinal, filtro.DataFinal, filtro.FormatosIds, filtro.PalavrasChavesIds,
                request.NumeroLinha, request.NumeroRegistros);
        }
    }
}
