namespace SME.ConectaFormacao.Dominio.Entidades
{
    /// <summary>
    /// Entidade para rastreamento de e-mails enviados via SMTP.
    /// Garante idempotência impedindo envio duplicado de mensagens.
    /// Utilizada principalmente para e-mails diretos sem vínculo com notificações do sistema.
    /// </summary>
    public class EmailEnviado : EntidadeBaseAuditavel
    {
        /// <summary>
        /// Chave única gerada via hash para garantir idempotência.
        /// Formato: SHA256({CorrelacaoId}-{Email}-{Titulo}-{DataHora:yyyyMMddHH})
        /// </summary>
        public string ChaveIdempotencia { get; set; } = string.Empty;

        public string EmailDestinatario { get; set; } = string.Empty;

        public string NomeDestinatario { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Hash SHA256 do conteúdo completo do e-mail.
        /// Permite identificar se o mesmo conteúdo foi enviado anteriormente.
        /// </summary>
        public string ConteudoHash { get; set; } = string.Empty;

        public DateTime EnviadoEm { get; set; }

        public long? NotificacaoUsuarioId { get; set; }

        public NotificacaoUsuario? NotificacaoUsuario { get; set; }

        public int TentativasEnvio { get; set; }

        public string? MensagemErro { get; set; }
        public bool Enviado { get; set; }
    }
}
