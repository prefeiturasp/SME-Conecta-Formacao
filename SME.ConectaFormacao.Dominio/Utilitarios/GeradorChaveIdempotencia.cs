using System.Security.Cryptography;
using System.Text;

namespace SME.ConectaFormacao.Dominio.Utilitarios
{
    /// <summary>
    /// Utilitário responsável por gerar chaves únicas de idempotência para envio de e-mails.
    /// Utiliza hash SHA256 para garantir unicidade e consistência.
    /// Segue princípios KISS mantendo lógica simples e direta.
    /// </summary>
    public static class GeradorChaveIdempotencia
    {
        /// <summary>
        /// Gera chave de idempotência baseada nos parâmetros do e-mail.
        /// Formato: SHA256({correlacaoId}-{email}-{titulo}-{janelaTemporal})
        /// </summary>
        public static string Gerar(
            string emailDestinatario,
            string titulo,
            Guid? correlacaoId = null,
            DateTime? janelaTemporal = null,
            bool comJanelaTemporal = true)
        {
            var (emailNormalizado, tituloNormalizado) = PrepararInformacoesEssenciais(emailDestinatario, titulo);

            var dataReferencia = janelaTemporal ?? DateTime.Now;
            var janela = dataReferencia.ToString("yyyyMMddHH");

            var correlacao = correlacaoId?.ToString() ?? "sem-correlacao";

            var chaveBase = comJanelaTemporal
                ? $"{correlacao}-{emailNormalizado}-{tituloNormalizado}-{janela}"
                : $"{correlacao}-{emailNormalizado}-{tituloNormalizado}";

            return GerarHashSHA256(chaveBase);
        }

        /// <summary>
        /// Gera chave de idempotência para notificações vinculadas a NotificacaoUsuario.
        /// Formato: SHA256({notificacaoId}-{notificacaoUsuarioId}-{email}-{titulo})
        /// </summary>
        public static string GerarParaNotificacao(
            long notificacaoId,
            long notificacaoUsuarioId,
            string emailDestinatario,
            string titulo)
        {
            var (emailNormalizado, tituloNormalizado) = PrepararInformacoesEssenciais(emailDestinatario, titulo);

            var chaveBase = $"{notificacaoId}-{notificacaoUsuarioId}-{emailNormalizado}-{tituloNormalizado}";

            return GerarHashSHA256(chaveBase);
        }

        private static string GerarHashSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static (string emailNormalizado, string tituloNormalizado) PrepararInformacoesEssenciais(string emailDestinatario, string titulo)
        {
            if (string.IsNullOrWhiteSpace(emailDestinatario))
                throw new ArgumentException("Email do destinatário é obrigatório", nameof(emailDestinatario));

            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título do e-mail é obrigatório", nameof(titulo));

            var emailNormalizado = emailDestinatario.Trim().ToLowerInvariant();
            var tituloNormalizado = titulo.Trim();

            return (emailNormalizado, tituloNormalizado);
        }
    }
}
