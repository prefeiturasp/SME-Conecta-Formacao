using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class CodafDeclaracoesProfile : Profile
    {
        public CodafDeclaracoesProfile()
        {
            CreateMap<FiltroListaMinhasDeclaracoesCodafDto, FiltroMinhasDeclaracoesCodafDto>()
                .ForMember(dest => dest.Pagina, opt => opt.MapFrom(src => src.NumeroPagina))
                .ForMember(dest => dest.TamanhoPagina, opt => opt.MapFrom(src => src.NumeroRegistros));
        }
    }
}