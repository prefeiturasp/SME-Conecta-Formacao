using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class NotificacaoProfile : Profile
    {
        public NotificacaoProfile()
        {
            // -> Notificações Gerais
            CreateMap<Notificacao, NotificacaoDTO>()
                .ForMember(dest => dest.CategoriaDescricao, opt => opt.MapFrom(o => o.Categoria.Nome()))
                .ForMember(dest => dest.TipoDescricao, opt => opt.MapFrom(o => o.Tipo.Nome()));

            CreateMap<Notificacao, NotificacaoPaginadoDTO>()
                .ForMember(dest => dest.CategoriaDescricao, opt => opt.MapFrom(o => o.Categoria.Nome()))
                .ForMember(dest => dest.TipoDescricao, opt => opt.MapFrom(o => o.Tipo.Nome()))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => o.Usuarios.First().Situacao))
                .ForMember(dest => dest.SituacaoDescricao, opt => opt.MapFrom(o => o.Usuarios.First().Situacao.Nome()));

            // -> SignalR e Push
            CreateMap<Notificacao, NotificacaoSignalRDTO>()
                .ForMember(dest => dest.Usuarios, opt => opt.MapFrom(o => o.Usuarios.Any() ? o.Usuarios.Select(s => s.Login) : ArraySegment<string>.Empty));

            // -> Destinatários de Notificação
            CreateMap<Usuario, NotificacaoUsuario>()
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(o => NotificacaoUsuarioSituacao.NaoLida));

            CreateMap<RetornoUsuarioLoginNomeDTO, NotificacaoUsuario>();

            // Pareceristas virando usuários de notificação
            CreateMap<PropostaPareceristaResumidoDTO, NotificacaoUsuario>();

            CreateMap<PropostaParecerista, PropostaPareceristaResumidoDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(o => o.NomeParecerista))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(o => o.RegistroFuncional));

            // -> E-mail
            CreateMap<NotificacaoUsuario, EnviarEmailDto>()
                .ForMember(dest => dest.NomeDestinatario, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.EmailDestinatario, opt => opt.MapFrom(src => src.Email));
        }
    }
}