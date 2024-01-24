using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargosFuncoesDresFuncionarioServicoEolQueryHandler : IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CursistaCargoServicoEol>>
    {
        private readonly IServicoEol _servicoEol;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterCargosFuncoesDresFuncionarioServicoEolQueryHandler(IServicoEol servicoEol, ICacheDistribuido cacheDistribuido)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<CursistaCargoServicoEol>> Handle(ObterCargosFuncoesDresFuncionarioServicoEolQuery request, CancellationToken cancellationToken)
        {
            var nomeCache = CacheDistribuidoNomes.CargosFuncoesDresEolFuncionario.Parametros(request.RegistroFuncional);
            return await _cacheDistribuido.ObterAsync(nomeCache, () => _servicoEol.ObterCargosFuncionadoPorRegistroFuncional(request.RegistroFuncional));
        }
    }
}
