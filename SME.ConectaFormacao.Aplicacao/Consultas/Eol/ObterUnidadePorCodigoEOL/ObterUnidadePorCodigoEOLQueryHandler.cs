using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUnidadePorCodigoEOLQueryHandler : IRequestHandler<ObterUnidadePorCodigoEOLQuery, UnidadeEol>
    {
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterUnidadePorCodigoEOLQueryHandler(IServicoEol servicoEol,ICacheDistribuido cacheDistribuido)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        private readonly IServicoEol _servicoEol;
        public async Task<UnidadeEol> Handle(ObterUnidadePorCodigoEOLQuery request, CancellationToken cancellationToken)
        {
            
            var ue = await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.UnidadeEol.Parametros(request.UnidadeCodigo), () => _servicoEol.ObterUnidadePorCodigoEol(request.UnidadeCodigo));
            if (ue.NomeUnidade.EhNulo())
                throw new NegocioException(MensagemNegocio.UNIDADE_NAO_LOCALIZADA_POR_CODIGO);

            return ue;
        }
    }
}