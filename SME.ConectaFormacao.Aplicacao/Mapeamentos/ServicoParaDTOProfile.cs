using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class ServicoParaDTOProfile : Profile
    {
        public ServicoParaDTOProfile()
        {
            CreateMap<AcessosUsuarioAutenticacaoRetorno, UsuarioAutenticacaoRetornoDTO>();
            CreateMap<AcessosPerfisUsuarioRetorno, UsuarioPerfisRetornoDTO>();
            CreateMap<AcessosPerfilUsuario, PerfilUsuarioDTO>();

            CreateMap<AcessosDadosUsuario, DadosUsuarioDTO>()
                .ForMember(dest => dest.Cpf, opt => opt.MapFrom(x => x.Cpf.AplicarMascara(@"000\.000\.000\-00")));

            CreateMap<AcessosGrupo, GrupoDTO>().ReverseMap();
        }
    }
}
