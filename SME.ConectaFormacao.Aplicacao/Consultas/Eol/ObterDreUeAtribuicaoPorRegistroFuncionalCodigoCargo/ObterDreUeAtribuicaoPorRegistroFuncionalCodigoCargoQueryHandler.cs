using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandler : IRequestHandler<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery, IEnumerable<DreUeAtribuicaoServicoEol>>
    {
        private readonly IServicoEol _servicoEol;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandler(IServicoEol servicoEol, ICacheDistribuido cacheDistribuido)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public Task<IEnumerable<DreUeAtribuicaoServicoEol>> Handle(ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery request, CancellationToken cancellationToken)
        {
            var nomeCache = CacheDistribuidoNomes.DreUeAtribuicaoFuncionarioCargo.Parametros(request.RegistroFuncional, request.CodigoCargo);
            return _cacheDistribuido.ObterAsync(nomeCache, () => _servicoEol.ObterDreUeAtribuicaoPorFuncionarioCargo(request.RegistroFuncional, long.Parse(request.CodigoCargo)));
        }
    }
}
