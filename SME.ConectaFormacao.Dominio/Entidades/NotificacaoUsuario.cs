using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class NotificacaoUsuario : EntidadeBaseAuditavel
    {
        public NotificacaoUsuario()
        {
            Situacao = NotificacaoUsuarioSituacao.NaoLida;
        }

        public Notificacao Notificacao { get; set; }
        public long NotificacaoId { get; set; }
        
        public Usuario Usuario { get; set; }
        public string Login { get; set; }
        
        public NotificacaoUsuarioSituacao Situacao { get; set; }
    }
}