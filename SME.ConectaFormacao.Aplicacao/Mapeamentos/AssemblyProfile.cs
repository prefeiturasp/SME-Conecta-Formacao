using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class AssemblyProfile : Profile
    {
        public AssemblyProfile()
        {
            // Esta classe serve apenas para forçar o carregamento do assembly para o AutoMapper.
        }
    }
}