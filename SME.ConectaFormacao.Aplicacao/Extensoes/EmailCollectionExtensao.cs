using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Extensoes
{
    public static class EmailCollectionExtensao
    {
        public static List<NotificacaoUsuario> RemoverDuplicatasPorEmail(this IEnumerable<NotificacaoUsuario> usuarios)
        {
            return usuarios == null
                ? []
                : [.. usuarios
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .GroupBy(u => u.Email.Trim().ToLowerInvariant())
                .Select(g => g.First())];
        }

        public static List<EnviarEmailDto> RemoverDuplicatasPorEmailDestinatario(this IEnumerable<EnviarEmailDto> emails)
        {
            return emails == null
                ? []
                : [.. emails
                .Where(e => !string.IsNullOrWhiteSpace(e.EmailDestinatario))
                .GroupBy(e => e.EmailDestinatario.Trim().ToLowerInvariant())
                .Select(g => g.First())];
        }
    }
}
