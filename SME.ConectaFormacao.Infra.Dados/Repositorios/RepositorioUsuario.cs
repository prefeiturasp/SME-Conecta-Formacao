﻿using Dapper;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioUsuario : RepositorioBaseAuditavel<Usuario>, IRepositorioUsuario
    {
        public RepositorioUsuario(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<Usuario> ObterPorLogin(string login)
        {
            var query = @"select 
                            id, 
                            login,                             
                            nome,                             
                            ultimo_login, 
                            expiracao_recuperacao_senha, 
                            token_recuperacao_senha,
                            criado_em, 
                            criado_por, 
                            alterado_em, 
                            alterado_por, 
                            criado_login, 
                            alterado_login
                          from usuario 
                          where login = @login";

            return conexao.Obter().QueryFirstOrDefaultAsync<Usuario>(query, new { login });
        }
    }
}
