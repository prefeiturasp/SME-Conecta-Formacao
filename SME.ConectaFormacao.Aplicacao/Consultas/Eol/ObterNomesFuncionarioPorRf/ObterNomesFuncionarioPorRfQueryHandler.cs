using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf
{
    public class ObterNomesFuncionarioPorRfQueryHandler(
        IServicoEol servicoEol,
        ICacheDistribuido cacheDistribuido) : IRequestHandler<ObterNomesFuncionarioPorRfQuery, FuncionarioNomesDto?>
    {
        public async Task<FuncionarioNomesDto?> Handle(ObterNomesFuncionarioPorRfQuery request, CancellationToken cancellationToken)
        {
            var chaveCache = CacheDistribuidoNomes.NomesUsuario.Parametros(request.Rf);
            var nomesFuncionario = await cacheDistribuido.ObterAsync(chaveCache, 
                async () => await servicoEol.ObterNomesFuncionarioPorRegistroFuncional(request.Rf));
            return nomesFuncionario;
        }
    }
}