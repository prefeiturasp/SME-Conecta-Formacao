using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class AreaPromotoraProfile : Profile
    {
        public AreaPromotoraProfile()
        {
            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()))
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre!.Nome));

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>()
                .ForMember(dest => dest.DreId, opt => opt.MapFrom(x => x.DreId))
                .ForMember(dest => dest.NomeDre, opt => opt.MapFrom(x => x.Dre!.Nome))
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })));

            CreateMap<AreaPromotora, AreaPromotoraDTO>()
                .ForMember(dst => dst.Emails, map => map.MapFrom(src => src.Email.Split(';', StringSplitOptions.None).Select(t => new AreaPromotoraEmailDTO { Email = t })))
                .ReverseMap()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => string.Join(";", x.Emails.Select(t => t.Email))));

            CreateMap<AreaPromotoraTelefone, AreaPromotoraTelefoneDTO>()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.Length > 10 ? x.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : x.Telefone.AplicarMascara(@"\(00\) 0000\-0000")))
                .ReverseMap()
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(x => x.Telefone.SomenteNumeros()));

            CreateMap<AreaPromotora, RetornoListagemDTO>()
               .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<AreaPromotora, PropostaAreaPromotoraDTO>();

            CreateMap<Dre, DreDTO>()
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(x => x.Nome));

            CreateMap<Dre, DreServicoEol>().ReverseMap();
        }
    }
}