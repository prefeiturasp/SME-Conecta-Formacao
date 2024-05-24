using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

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
    }
}
