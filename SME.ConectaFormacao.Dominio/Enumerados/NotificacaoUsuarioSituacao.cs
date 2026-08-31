using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum NotificacaoUsuarioSituacao
    {
        [Display(Name = "Não lida")]
        NaoLida = 1,

        [Display(Name = "Lida")]
        Lida = 2
    }

    [ExcludeFromCodeCoverage]
    public static class NotificacaoUsuarioSituacaoExtensao
    {
        public static bool EhNaoLida(this NotificacaoUsuarioSituacao notificacaoUsuarioSituacao)
        {
            return notificacaoUsuarioSituacao == NotificacaoUsuarioSituacao.NaoLida;
        }

        public static bool EhLida(this NotificacaoUsuarioSituacao notificacaoUsuarioSituacao)
        {
            return notificacaoUsuarioSituacao == NotificacaoUsuarioSituacao.Lida;
        }
    }
}