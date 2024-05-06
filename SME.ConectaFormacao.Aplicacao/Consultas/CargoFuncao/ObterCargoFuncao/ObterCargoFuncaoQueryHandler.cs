using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoQueryHandler : IRequestHandler<ObterCargoFuncaoQuery, IEnumerable<CargoFuncaoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterCargoFuncaoQueryHandler(IMapper mapper, IRepositorioCargoFuncao repositorioCargoFuncao, ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<CargoFuncaoDTO>> Handle(ObterCargoFuncaoQuery request, CancellationToken cancellationToken)
        {
            var nomeCache = CacheDistribuidoNomes.CargoFuncao.Parametros(request.Tipo, request.ExibirOutros);

            var cargosFuncoes = await _cacheDistribuido.ObterAsync(nomeCache, () => _repositorioCargoFuncao.ObterIgnorandoExcluidosPorTipo(request.Tipo, request.ExibirOutros));

            return _mapper.Map<IEnumerable<CargoFuncaoDTO>>(cargosFuncoes);
        }
    }
}
