using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public static class UsuarioAdminDfMock
    {
        public static IEnumerable<RetornoUsuarioLoginNomeDTO> GerarListaUsuariosAdminDf()
        {
            var usuariosPerfis = UsuarioPerfilServicoEolMock.UsuariosPerfis;
            if (usuariosPerfis == null)
                return Enumerable.Empty<RetornoUsuarioLoginNomeDTO>();

            return usuariosPerfis.Where(c => c.Perfil == Perfis.ADMIN_DF).Select(usuarioPerfil =>
                new RetornoUsuarioLoginNomeDTO { Login = usuarioPerfil.Login.ToString().PadLeft(7, '0'), Nome = usuarioPerfil.Nome }).ToList();
        }
    }
}