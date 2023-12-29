using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaFuncoesEspecificasPorIdQueryHandler : IRequestHandler<ObterPropostaFuncoesEspecificasPorIdQuery, IEnumerable<PropostaFuncaoEspecifica>>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaFuncoesEspecificasPorIdQueryHandler(ICacheDistribuido cacheDistribuido, IRepositorioProposta repositorioProposta)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaFuncaoEspecifica>> Handle(ObterPropostaFuncoesEspecificasPorIdQuery request, CancellationToken cancellationToken)
        {
            var nomeChave = CacheDistribuidoNomes.PropostaFuncaoEspecifica.Parametros(request.PropostaId);
            return await _cacheDistribuido.ObterAsync(nomeChave, () => _repositorioProposta.ObterFuncoesEspecificasPorId(request.PropostaId));
        }
    }
}
