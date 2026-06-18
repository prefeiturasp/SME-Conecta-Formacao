using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafSuplementarProfile : Profile
    {
        public CodafSuplementarProfile()
        {
            CreateMap<CodafListaPresenca, CodafSuplementarDetalhadoDto>()
                .ForMember(dest => dest.NomeFormacao, opt => opt.MapFrom(src => src.Proposta.NomeFormacao))
                .ForMember(dest => dest.CodigoFormacao, opt => opt.MapFrom(src => src.Proposta.Id))
                .ForMember(dest => dest.NumeroHomologacao, opt => opt.MapFrom(src => src.Proposta.NumeroHomologacao))
                .ForMember(dest => dest.Retificacoes, opt => opt.MapFrom(src => src.CodafRetificacoes))
                .ForMember(dest => dest.Anexos, opt => opt.MapFrom(src => src.CodafAnexos))
                ;
            CreateMap<CodafSuplementarRetificacao, CodafSuplementarRetificacaoDto>();
            CreateMap<CodafAnexoSalvarDto, CodafSuplementarAnexo>()
                .ForMember(dest => dest.Extensao, opt => opt.MapFrom(src => src.NomeArquivo.Substring(src.NomeArquivo.LastIndexOf('.') + 1)))
                ;
            CreateMap<CodafSuplementarAnexo, CodafSuplementarAnexoDto>().ForMember(dest => dest.UrlDownload, opt => opt.Ignore());
        }
    }
}