namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface IValidadorPermissaoCodaf
    {
        Task<bool> ValidarSeUsuarioEhCriador(Dominio.Entidades.Usuario usuarioLogado, long codafListaPresencaId);
        Task<bool> UsuarioPossuiPerfilAdminOuEMFORPEF(Guid usuarioPerfil);
        Task<Guid> BuscarPerfilUsuario();
    }
}