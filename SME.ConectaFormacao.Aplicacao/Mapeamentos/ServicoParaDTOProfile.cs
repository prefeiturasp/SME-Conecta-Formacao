using AutoMapper;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Infra.Servicos.Acessos;

namespace SME.ConectaFormacao.Aplicacao.Mapeamentos
{
    public class ServicoParaDTOProfile : Profile
    {
        protected ServicoParaDTOProfile()
        {
            this.CreateMap<AcessosUsuarioAutenticacaoRetorno, UsuarioAutenticacaoRetornoDTO>();
        }
    }
}
