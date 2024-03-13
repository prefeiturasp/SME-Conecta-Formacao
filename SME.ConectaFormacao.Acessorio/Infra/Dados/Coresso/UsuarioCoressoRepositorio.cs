using Dapper;
using System.Data;

namespace SME.ConectaFormacao.Acessorio.Infra.Dados.Coresso;

public class UsuarioCoressoRepositorio
{
    private readonly IDbConnection conexao;

    public UsuarioCoressoRepositorio(IDbConnection conexao)
    {
        this.conexao = conexao ?? throw new ArgumentNullException(nameof(conexao));
    }

    public Task<int> AtualizarSenhaUsuario(string login, string senha)
    {
        var query = @"update sys_usuario set 
                        senha_antiga = usu_senha + ' | ' + cast(usu_criptografia as varchar(5)),
                        usu_senha = @senha, 
                        usu_criptografia = 1 
                      where usu_login = @login";

        return conexao.ExecuteAsync(query, new { login, senha });
    }
}