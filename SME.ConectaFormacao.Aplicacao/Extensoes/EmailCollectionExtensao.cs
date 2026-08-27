using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Extensoes
{
    public static class EmailCollectionExtensao
    {
        /// <summary>
        /// Remove duplicatas de uma lista de NotificacaoUsuario baseando-se no endereço de e-mail.
        /// Ignora e-mails vazios ou nulos.
        /// A comparação é case-insensitive e remove espaços.
        /// Retorna apenas a primeira ocorrência de cada e-mail único.
        /// </summary>
        /// <param name="usuarios">Lista de usuários de notificação</param>
        /// <returns>Lista de usuários sem duplicatas de e-mail</returns>
        public static List<NotificacaoUsuario> RemoverDuplicatasPorEmail(this IEnumerable<NotificacaoUsuario> usuarios)
        {
            if (usuarios == null)
                return new List<NotificacaoUsuario>();

            return usuarios
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .GroupBy(u => u.Email.Trim().ToLowerInvariant())
                .Select(g => g.First())
                .ToList();
        }

        /// <summary>
        /// Remove duplicatas de uma lista de NotificacaoUsuario baseando-se no endereço de e-mail,
        /// filtrando apenas usuários com e-mail preenchido usando o método EstaPreenchido().
        /// A comparação é case-insensitive e remove espaços.
        /// Retorna apenas a primeira ocorrência de cada e-mail único.
        /// </summary>
        /// <param name="usuarios">Lista de usuários de notificação</param>
        /// <returns>Lista de usuários sem duplicatas de e-mail</returns>
        public static List<NotificacaoUsuario> RemoverDuplicatasPorEmailPreenchido(this IEnumerable<NotificacaoUsuario> usuarios)
        {
            if (usuarios == null)
                return new List<NotificacaoUsuario>();

            return usuarios
                .Where(u => u.Email.EstaPreenchido())
                .GroupBy(u => u.Email.Trim().ToLowerInvariant())
                .Select(g => g.First())
                .ToList();
        }

        /// <summary>
        /// Remove duplicatas de uma lista de EnviarEmailDto baseando-se no endereço de e-mail do destinatário.
        /// Ignora e-mails vazios ou nulos.
        /// A comparação é case-insensitive e remove espaços.
        /// Retorna apenas a primeira ocorrência de cada e-mail único.
        /// </summary>
        /// <param name="emails">Lista de DTOs de envio de e-mail</param>
        /// <returns>Lista de DTOs sem duplicatas de e-mail</returns>
        public static List<EnviarEmailDto> RemoverDuplicatasPorEmailDestinatario(this IEnumerable<EnviarEmailDto> emails)
        {
            if (emails == null)
                return new List<EnviarEmailDto>();

            return emails
                .Where(e => !string.IsNullOrWhiteSpace(e.EmailDestinatario))
                .GroupBy(e => e.EmailDestinatario.Trim().ToLowerInvariant())
                .Select(g => g.First())
                .ToList();
        }
    }
}
