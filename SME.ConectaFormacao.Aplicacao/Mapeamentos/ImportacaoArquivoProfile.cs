using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class ImportacaoArquivoProfile : Profile
    {
        public ImportacaoArquivoProfile()
        {
            CreateMap<ImportacaoArquivoDTO, ImportacaoArquivo>().ReverseMap();
            CreateMap<ImportacaoArquivoRegistroDto, ImportacaoArquivoRegistro>().ReverseMap();
        }
    }
}