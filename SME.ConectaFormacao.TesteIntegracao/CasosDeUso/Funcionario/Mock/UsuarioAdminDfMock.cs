using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public static class UsuarioAdminDfMock
    {
        public static IEnumerable<UsuarioAdminDfDTO> GerarListaUsuariosAdminDf()
        {
            var usuariosPerfis = UsuarioPerfilServicoEolMock.UsuariosPerfis;
            if (usuariosPerfis == null)
                return Enumerable.Empty<UsuarioAdminDfDTO>();

            return usuariosPerfis.Where(c => c.Perfil == Perfis.ADMIN_DF).Select(usuarioPerfil =>
                new UsuarioAdminDfDTO { Rf = usuarioPerfil.Login, Nome = usuarioPerfil.Nome }).ToList();
        }        
    }
}