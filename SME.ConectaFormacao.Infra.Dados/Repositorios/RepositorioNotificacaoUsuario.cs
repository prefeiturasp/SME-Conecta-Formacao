using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioNotificacaoUsuario : RepositorioBaseAuditavel<NotificacaoUsuario>, IRepositorioNotificacaoUsuario
    {
        public RepositorioNotificacaoUsuario(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task InserirUsuarios(IDbTransaction transacao, IEnumerable<NotificacaoUsuario> usuarios, long notificacaoId)
        {
            foreach (var usuario in usuarios)
            {
                PreencherAuditoriaCriacao(usuario);

                usuario.NotificacaoId = notificacaoId;
                usuario.Situacao = NotificacaoUsuarioSituacao.NaoLida;
                await conexao.Obter().InsertAsync(usuario, transacao);
            }
        }

        public Task<NotificacaoUsuario> ObterNotificacaoUsuario(long notificacaoId, string login)
        {
            var query = @"select 
                            id, 
                            notificacao_id, 
                            login,
                            nome,
                            email,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from notificacao_usuario
                        where not excluido and notificacao_id = @notificacaoId and login = @login";

            return conexao.Obter().QueryFirstOrDefaultAsync<NotificacaoUsuario>(query, new { notificacaoId, login });
        }
    }
}
