using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Dre.ObterDreTodos
{
    public class ObterDreTodosQueryHandler : IRequestHandler<ObterDreTodosQuery, Dominio.Entidades.Dre>
    {
        private readonly IRepositorioDre _repositorioDre;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterDreTodosQueryHandler(IRepositorioDre repositorioDre, ICacheDistribuido cacheDistribuido)
        {
            _repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<Dominio.Entidades.Dre> Handle(ObterDreTodosQuery request, CancellationToken cancellationToken)
        {
            return await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.DreTodos, () => _repositorioDre.ObterDreTodos());
        }
    }
}
