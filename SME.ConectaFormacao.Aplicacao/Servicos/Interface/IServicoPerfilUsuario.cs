using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Servicos.Interface
{
    public interface IServicoPerfilUsuario : IServicoAplicacao
    {
        Task<RetornoPerfilUsuarioDTO> ObterPerfisUsuario(string login);
    }
}
