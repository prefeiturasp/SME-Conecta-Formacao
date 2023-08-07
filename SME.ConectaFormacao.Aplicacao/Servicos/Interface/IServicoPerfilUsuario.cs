using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Infra.Servicos.Acessos;

namespace SME.ConectaFormacao.Aplicacao.Servicos.Interface
{
    public interface IServicoPerfilUsuario : IServicoAplicacao
    {
        Task<AcessosRetornoPerfilUsuario> ObterPerfisUsuario(string login);
    }
}
