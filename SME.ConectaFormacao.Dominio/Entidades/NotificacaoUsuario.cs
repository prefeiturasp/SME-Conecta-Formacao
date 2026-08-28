using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class NotificacaoUsuario : EntidadeBaseAuditavel
    {
        public NotificacaoUsuario()
        {
            Situacao = NotificacaoUsuarioSituacao.NaoLida;
            Notificacao = null!;
            Login = null!;
            Nome = null!;
            Email = null!;
        }

        public NotificacaoUsuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
            Notificacao = null!;
            Login = null!;
        }

        public NotificacaoUsuario(string login, string nome, string email)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Notificacao = null!;
        }

        public Notificacao Notificacao { get; set; }
        public long NotificacaoId { get; set; }

        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public NotificacaoUsuarioSituacao Situacao { get; set; }

        public DateTime? EmailEnviadoEm { get; set; }

        public bool EmailEnviado { get; set; }

        /// <summary>
        /// Hash único para idempotência do envio de e-mail.
        /// Formato: SHA256({NotificacaoId}-{UsuarioId}-{Email}-{Titulo})
        /// </summary>
        public string? EmailHash { get; set; }

        public int TentativasEnvioEmail { get; set; }

        public string? EmailErro { get; set; }
    }
}