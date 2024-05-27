using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class NotificacaoUsuario : EntidadeBaseAuditavel
    {
        public NotificacaoUsuario()
        {
            Situacao = NotificacaoUsuarioSituacao.NaoLida;
        }

        public NotificacaoUsuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }
        
        public NotificacaoUsuario(string login, string nome, string email)
        {
            Login = login;
            Nome = nome;
            Email = email;
        }

        public Notificacao Notificacao { get; set; }
        public long NotificacaoId { get; set; }
        
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        
        public NotificacaoUsuarioSituacao Situacao { get; set; }
    }
}