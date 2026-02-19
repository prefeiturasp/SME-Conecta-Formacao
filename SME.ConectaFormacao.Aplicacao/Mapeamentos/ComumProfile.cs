using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class ComumProfile : Profile
    {
        public ComumProfile()
        {
            CreateMap<UsuarioAcessibilidade, UsuarioAcessibilidadeDto>()
                .ForMember(dest => dest.Salvar, opt => opt.MapFrom(x => !x.Excluido))
                .ReverseMap()
                .ForMember(dest => dest.Excluido, opt => opt.MapFrom(x => !x.Salvar))
                .ForMember(dest => dest.DescricaoDeficiencia, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.DescricaoDeficiencia) ? x.DescricaoDeficiencia.Trim() : null))
                .ForMember(dest => dest.DescricaoAdaptacao, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.DescricaoAdaptacao) ? x.DescricaoAdaptacao.Trim() : null));
        }
    }
}