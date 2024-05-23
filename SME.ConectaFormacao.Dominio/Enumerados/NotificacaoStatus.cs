using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum NotificacaoStatus
    {
        [Display(Name = "Não lida")]
        NaoLida = 1,

        [Display(Name = "Lida")]
        Lida = 2
    }
}