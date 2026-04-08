using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorIdQueryHandler : IRequestHandler<ObterPropostaPorIdQuery, Proposta>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPorIdQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<Proposta> Handle(ObterPropostaPorIdQuery request, CancellationToken cancellationToken)
        {
            var nomeChave = CacheDistribuidoNomes.Proposta.Parametros(request.Id);
            return await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterPorId(request.Id));
        }
    }
}