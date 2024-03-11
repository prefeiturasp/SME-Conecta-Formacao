using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoOutrosQueryHandler : IRequestHandler<ObterCargoFuncaoOutrosQuery, CargoFuncao>
    {
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterCargoFuncaoOutrosQueryHandler(IRepositorioCargoFuncao repositorioCargoFuncao, ICacheDistribuido cacheDistribuido)
        {
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<CargoFuncao> Handle(ObterCargoFuncaoOutrosQuery request, CancellationToken cancellationToken)
        {
            return await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.CargoFuncaoOutros, () => _repositorioCargoFuncao.ObterCargoFuncaoOutros());
        }
    }
}
