using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class NotificacaoUsuarioMap : BaseMapAuditavel<NotificacaoUsuario>
    {
        public NotificacaoUsuarioMap()
        {
            ToTable("notificacao_usuario");
            Map(c => c.NotificacaoId).ToColumn("notificacao_id");
            Map(c => c.Login).ToColumn("login");
            Map(c => c.Status).ToColumn("status");
            
            Map(c => c.Nome).Ignore();
            Map(c => c.Notificacao).Ignore();
        }
    }
}
