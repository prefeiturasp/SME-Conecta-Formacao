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
        
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        
        public NotificacaoUsuarioSituacao Situacao { get; set; }
    }
}