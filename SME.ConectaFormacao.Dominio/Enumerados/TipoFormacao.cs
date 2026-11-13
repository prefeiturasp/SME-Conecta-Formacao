using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoFormacao
    {
        [Display(Name = "Curso")]
        Curso = 1,
        [Display(Name = "Evento")]
        Evento = 2,
        [Display(Name = "Conferência")]
        Conferencia = 3
    }
}
