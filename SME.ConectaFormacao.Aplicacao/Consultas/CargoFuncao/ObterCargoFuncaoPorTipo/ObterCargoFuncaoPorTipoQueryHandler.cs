using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoPorTipoQueryHandler(IMapper mapper, IRepositorioCargoFuncao repositorioCargoFuncao, ICacheDistribuido cacheDistribuido) : IRequestHandler<ObterCargoFuncaoPorTipoQuery, IEnumerable<CargoFuncaoDto>>
    {
        public async Task<IEnumerable<CargoFuncaoDto>> Handle(ObterCargoFuncaoPorTipoQuery request, CancellationToken cancellationToken)
        {
            var nomeCache = CacheDistribuidoNomes.CargoFuncao.Parametros(request.Tipo, request.ExibirOutros);

            var cargosFuncoes = await cacheDistribuido.ObterAsync(nomeCache, () => repositorioCargoFuncao.ObterIgnorandoExcluidosPorTipo(request.Tipo, request.ExibirOutros));

            return mapper.Map<IEnumerable<CargoFuncaoDto>>(cargosFuncoes);
        }
    }
}
