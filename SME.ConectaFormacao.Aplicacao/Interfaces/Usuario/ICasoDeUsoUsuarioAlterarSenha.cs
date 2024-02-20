using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarSenha
    {
        Task<bool> Executar(string login, AlterarSenhaUsuarioDTO alterarSenhaUsuarioDto);
    }
}
