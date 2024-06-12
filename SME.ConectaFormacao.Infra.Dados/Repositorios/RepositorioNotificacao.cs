using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioNotificacao : RepositorioBaseAuditavel<Notificacao>, IRepositorioNotificacao
    {
        public RepositorioNotificacao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<long> ObterTotalNaoLidoPorUsuario(string login)
        {
            var situacao = NotificacaoUsuarioSituacao.NaoLida;

            var query = @"select count(1) 
                          from notificacao_usuario nu 
                          inner join notificacao n on n.id = nu.notificacao_id and not n.excluido
                          where not nu.excluido and nu.login = @login and nu.situacao = @situacao";

            return conexao.Obter().ExecuteScalarAsync<long>(query, new { login, situacao });
        }
        public Task<int> ObterTotalNotificacao(string login, long? id, string? titulo, NotificacaoCategoria? categoria, NotificacaoTipo? tipo, NotificacaoUsuarioSituacao? situacao)
        {
            var query = new StringBuilder();
            query.AppendLine(" select count(1) ");
            query.AppendLine("from notificacao_usuario nu ");
            query.AppendLine("inner join notificacao n on n.id = nu.notificacao_id and not n.excluido ");
            query.AppendLine("where not nu.excluido and nu.login = @login ");

            if (id.HasValue)
                query.AppendLine(" and nu.notificacao_id = @id");

            if (titulo.EstaPreenchido())
            {
                titulo = "%" + titulo.ToLower() + "%";
                query.Append(" AND lower(n.titulo) LIKE @titulo ");
            }

            if (categoria.HasValue)
                query.AppendLine(" and n.categoria = @categoria");

            if (tipo.HasValue)
                query.AppendLine(" and n.tipo = @tipo");

            if (situacao.HasValue)
                query.AppendLine(" and nu.situacao = @situacao");

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { login, id, titulo, categoria, tipo, situacao });
        }

        public Task<IEnumerable<Notificacao>> ObterNotificacaoPaginada(string login, long? id, string? titulo, NotificacaoCategoria? categoria, NotificacaoTipo? tipo, NotificacaoUsuarioSituacao? situacao, int numeroRegistros, int quantidadeRegistrosIgnorados)
        {
            var query = new StringBuilder();
            query.AppendLine(" select n.id, n.titulo, n.categoria, n.tipo, nu.id, nu.situacao ");
            query.AppendLine("from notificacao_usuario nu ");
            query.AppendLine("inner join notificacao n on n.id = nu.notificacao_id and not n.excluido ");
            query.AppendLine("where not nu.excluido and nu.login = @login ");

            if (id.HasValue)
                query.AppendLine(" and nu.notificacao_id = @id");

            if (titulo.EstaPreenchido())
            {
                titulo = "%" + titulo.ToLower() + "%";
                query.Append(" AND lower(n.titulo) LIKE @titulo ");
            }

            if (categoria.HasValue)
                query.AppendLine(" and n.categoria = @categoria");

            if (tipo.HasValue)
                query.AppendLine(" and n.tipo = @tipo");

            if (situacao.HasValue)
                query.AppendLine(" and nu.situacao = @situacao");

            query.Append(" order by n.id desc ");
            query.Append(" limit @numeroRegistros offset @quantidadeRegistrosIgnorados");

            return conexao.Obter().QueryAsync<Notificacao, NotificacaoUsuario, Notificacao>(query.ToString(),
                (notificacao, notificacaoUsuario) =>
                {
                    notificacao.Usuarios = new NotificacaoUsuario[] { notificacaoUsuario };
                    return notificacao;
                },
                new { login, id, titulo, categoria, tipo, situacao, numeroRegistros, quantidadeRegistrosIgnorados }, splitOn: "id, id");
        }
    }
}
