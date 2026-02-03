using MediatR;
using Microsoft.Extensions.Logging;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterParametroSistemaPorTipoEAnoQueryHandler(
        IRepositorioParametroSistema repositorioParametroSistema,
        ICacheDistribuido cacheDistribuido,
        ILogger<ObterParametroSistemaPorTipoEAnoQueryHandler> logger) : IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>
    {
        public async Task<ParametroSistema> Handle(ObterParametroSistemaPorTipoEAnoQuery request,
            CancellationToken cancellationToken)
        {
            var chave = string.Format(CacheDistribuidoNomes.ParametroSistemaTipo, request.TipoParametroSistema);
            var retorno = await cacheDistribuido.ObterAsync(chave, () => repositorioParametroSistema.ObterParametroPorTipoEAnoAsync(request.TipoParametroSistema, request.Ano));

            if (retorno is null)
            {
                logger.LogWarning(MensagemNegocio.PARAMETRO_X_NAO_ENCONTRADO_PARA_ANO_Y, request.TipoParametroSistema, request.Ano);
                retorno = await cacheDistribuido.ObterAsync(chave, () => repositorioParametroSistema.ObterParametroPorTipoMaisRecenteAsync(request.TipoParametroSistema));

                if (retorno is null)
                    throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_X_NAO_ENCONTRADO_PARA_ANO_Y, request.TipoParametroSistema, request.Ano));
            }

            return retorno;
        }
    }
}
