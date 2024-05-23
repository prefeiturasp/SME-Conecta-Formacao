using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class NotificacaoUsuarioMap : BaseMapAuditavel<NotificacaoUsuario>
    {
        public NotificacaoUsuarioMap()
        {
            ToTable("notificacao_usuario");
            Map(c => c.NotificacaoId).ToColumn("notificacao_id");
            Map(c => c.RegistroFuncional).ToColumn("registro_funcional");
            Map(c => c.Status).ToColumn("status");
            
            Map(c => c.Notificacao).Ignore();
            Map(c => c.Usuario).Ignore();
        }
    }
}
