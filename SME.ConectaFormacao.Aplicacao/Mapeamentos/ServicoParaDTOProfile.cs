using AutoMapper;
using SME.ConectaFormacao.Aplicacao.DTOS;
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
        }
    }
}
