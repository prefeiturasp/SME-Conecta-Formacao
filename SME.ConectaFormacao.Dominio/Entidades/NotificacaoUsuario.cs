using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class NotificacaoUsuario : EntidadeBaseAuditavel
    {
        public NotificacaoUsuario()
        {
            Status = NotificacaoStatus.NaoLida;
        }

        public Notificacao Notificacao { get; set; }
        public long NotificacaoId { get; set; }
        
        public string Login { get; set; }
        public string Nome { get; set; }
        
        public NotificacaoStatus Status { get; set; }
    }
}