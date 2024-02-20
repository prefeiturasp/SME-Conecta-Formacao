using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterParametroSistemaPorTipoEAnoQueryHandler : IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>
    {
        private readonly IRepositorioParametroSistema repositorioParametroSistema;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterParametroSistemaPorTipoEAnoQueryHandler(IRepositorioParametroSistema repositorioParametroSistema, ICacheDistribuido cacheDistribuido)
        {
            this.repositorioParametroSistema = repositorioParametroSistema ?? throw new ArgumentNullException(nameof(repositorioParametroSistema));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<ParametroSistema> Handle(ObterParametroSistemaPorTipoEAnoQuery request,
            CancellationToken cancellationToken)
        {
            var chave = string.Format(CacheDistribuidoNomes.ParametroSistemaTipo, request.TipoParametroSistema);
            return await _cacheDistribuido.ObterAsync(chave, () => repositorioParametroSistema.ObterParametroPorTipoEAno(request.TipoParametroSistema, request.Ano));
        }
    }
}
