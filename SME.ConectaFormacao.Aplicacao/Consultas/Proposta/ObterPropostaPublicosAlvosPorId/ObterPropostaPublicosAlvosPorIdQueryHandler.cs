using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPublicosAlvosPorIdQueryHandler : IRequestHandler<ObterPropostaPublicosAlvosPorIdQuery, IEnumerable<PropostaPublicoAlvo>>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPublicosAlvosPorIdQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaPublicoAlvo>> Handle(ObterPropostaPublicosAlvosPorIdQuery request, CancellationToken cancellationToken)
        {
            var nomeChave = CacheDistribuidoNomes.PropostaPublicoAlvo.Parametros(request.PropostaId);
            return await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterPublicoAlvoPorId(request.PropostaId));
        }
    }
}
