using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class DominioParaDTOProfile : Profile
    {
        public DominioParaDTOProfile()
        {
            CreateMap<EntidadeBaseAuditavel, AuditoriaDTO>();

            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()));

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>();

            CreateMap<AreaPromotoraDTO, AreaPromotora>().ReverseMap();

            CreateMap<AreaPromotoraTelefone, AreaPromotoraTelefoneDTO>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.Length > 10 ? x.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : x.Telefone.AplicarMascara(@"\(00\) 0000\-0000")));

            CreateMap<AreaPromotoraTelefoneDTO, AreaPromotoraTelefone>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.SomenteNumeros()));
        }
    }
}
