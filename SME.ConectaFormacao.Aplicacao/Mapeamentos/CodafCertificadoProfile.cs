using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafCertificadoProfile : Profile
    {
        public CodafCertificadoProfile()
        {
            CreateMap<FiltroListaMeusCertificadosCodafDto, FiltroMeusCertificadosCodafDto>()
                .ForMember(dest => dest.Pagina, opt => opt.MapFrom(src => src.NumeroPagina))
                .ForMember(dest => dest.TamanhoPagina, opt => opt.MapFrom(src => src.NumeroRegistros));

            CreateMap<FiltroListaTodosCertificadosCodafDto, FiltroListagemTodosCertificadosCodafDto>()
                .ForMember(dest => dest.Pagina, opt => opt.MapFrom(src => src.NumeroPagina))
                .ForMember(dest => dest.TamanhoPagina, opt => opt.MapFrom(src => src.NumeroRegistros));
        }
    }
}