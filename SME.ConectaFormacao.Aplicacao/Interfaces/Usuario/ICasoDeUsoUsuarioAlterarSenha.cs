using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioAlterarSenha
    {
        Task<bool> Executar(string login, AlterarSenhaUsuarioDTO alterarSenhaUsuarioDto);
    }
}
