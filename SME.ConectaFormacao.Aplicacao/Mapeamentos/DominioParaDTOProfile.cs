using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
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

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>()
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })));

            CreateMap<AreaPromotoraDTO, AreaPromotora>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => string.Join(";", x.Emails.Select(t => t.Email))))
                .ReverseMap()
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })));

            CreateMap<AreaPromotoraTelefone, AreaPromotoraTelefoneDTO>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.Length > 10 ? x.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : x.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))
                .ReverseMap()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.SomenteNumeros()));
            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDTO>();
            CreateMap<CriterioValidacaoInscricao, CriterioValidacaoInscricaoDTO>();
        }
    }
}
