using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public static class ExtensoesMapeamentoCodaf
    {
        /// <summary>
        /// Aplica os mapeamentos comuns de filtros de paginação, delegando ao AutoMapper a conversão implícita de tipos numéricos para string.
        /// </summary>
        public static IMappingExpression<TOrigem, TDestino> AplicarMapeamentoFiltroPaginacao<TOrigem, TDestino>(
            this IMappingExpression<TOrigem, TDestino> mapeamento)
        {
            return mapeamento
                .ForMember("CodigoFormacao", opt => opt.MapFrom("CodigoFormacao"))
                .ForMember("NumeroHomologacao", opt => opt.MapFrom("NumeroHomologacao"))
                .ForMember("Pagina", opt => opt.MapFrom("NumeroPagina"))
                .ForMember("TamanhoPagina", opt => opt.MapFrom("NumeroRegistros"));
        }
    }
}