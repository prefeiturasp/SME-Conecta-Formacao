using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.DTOS;
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

            CreateMap<AcessosGrupo, GrupoDTO>();
        }
    }
}
