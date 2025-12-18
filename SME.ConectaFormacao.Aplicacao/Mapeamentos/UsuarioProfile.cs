using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            // -> Gestão de Usuário Básico
            CreateMap<DadosUsuarioDTO, Usuario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome.Trim()));

            CreateMap<UsuarioExternoDTO, Usuario>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome.Trim()))
                .ReverseMap();

            CreateMap<Usuario, DadosLoginUsuarioDto>();

            CreateMap<RetornoUsuarioCpfNomeDTO, Usuario>().ReverseMap();

            // -> Integração EOL
            CreateMap<RetornoUsuarioCpfNomeDTO, CursistaResumidoServicoEol>().ReverseMap();
            CreateMap<CursistaServicoEol, CursistaServicoEol>().ReverseMap();

            // -> Inscrição Automática (Criação de usuário via inscrição)
            CreateMap<Usuario, InscricaoAutomaticaDTO>()
                .ForMember(dest => dest.UsuarioRf, opt => opt.MapFrom(o => o.Login))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.UsuarioCpf, opt => opt.MapFrom(o => o.Cpf))
                .ReverseMap()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(o => TipoUsuario.Interno))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => SituacaoUsuario.Ativo));

            // -> Rede Parceria
            CreateMap<Usuario, UsuarioRedeParceriaPaginadoDTO>()
                .ForMember(dest => dest.AreaPromotora, opt => opt.MapFrom(o => o.AreaPromotora.Nome))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(o => o.Email))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(o => !string.IsNullOrWhiteSpace(o.Telefone) ? o.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : string.Empty))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Situacao.Nome()));

            CreateMap<Usuario, UsuarioRedeParceriaDTO>()
                .ForMember(dest => dest.AreaPromotoraId, opt => opt.MapFrom(o => o.AreaPromotoraId))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.Nome))
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(o => o.Cpf))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(o => o.Email))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(o => o.Telefone));
        }
    }
}