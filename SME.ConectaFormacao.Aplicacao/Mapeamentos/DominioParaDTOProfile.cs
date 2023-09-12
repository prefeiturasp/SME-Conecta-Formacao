using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
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

            // -> Area Promotora
            CreateMap<AreaPromotora, AreaPromotoraPaginadaDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(x => x.Tipo.Nome()));

            CreateMap<AreaPromotora, AreaPromotoraCompletoDTO>()
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

            // -> Proposta
            CreateMap<RoteiroPropostaFormativa, RoteiroPropostaFormativaDTO>();
            CreateMap<CargoFuncao, CargoFuncaoDTO>();
            CreateMap<CriterioValidacaoInscricao, CriterioValidacaoInscricaoDTO>();
            CreateMap<Proposta, PropostaCompletoDTO>();
            CreateMap<Proposta, PropostaDTO>().ReverseMap();
            CreateMap<PropostaCriterioValidacaoInscricao, PropostaCriterioValidacaoInscricaoDTO>().ReverseMap();
            CreateMap<PropostaFuncaoEspecifica, PropostaFuncaoEspecificaDTO>().ReverseMap();
            CreateMap<PropostaVagaRemanecente, PropostaVagaRemanecenteDTO>().ReverseMap();
            CreateMap<PropostaPublicoAlvo, PropostaPublicoAlvoDTO>().ReverseMap();
        }
    }
}
