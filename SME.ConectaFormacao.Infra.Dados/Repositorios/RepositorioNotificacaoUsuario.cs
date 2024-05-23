using System.Data;
using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using Dommel;

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
                await conexao.Obter().InsertAsync(usuario,transacao);
            }
        }
    }
}
