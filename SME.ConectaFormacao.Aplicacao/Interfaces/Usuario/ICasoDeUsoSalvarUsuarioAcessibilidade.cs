using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoSalvarUsuarioAcessibilidade
    {
        Task<Resultado> ExecutarAsync(string login, UsuarioAcessibilidadeDto usuarioAcessibilidadeDto);
    }
}
