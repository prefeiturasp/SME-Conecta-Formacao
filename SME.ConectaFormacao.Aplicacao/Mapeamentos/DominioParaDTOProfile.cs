using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class DominioParaDTOProfile : Profile
    {
        public DominioParaDTOProfile()
        {
            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()));

            CreateMap<AreaPromotoraDTO, AreaPromotora>().ReverseMap();
            CreateMap<AreaPromotoraTelefoneDTO, AreaPromotoraTelefone>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.SomenteNumeros()))
                .ReverseMap();
        }
    }
}
