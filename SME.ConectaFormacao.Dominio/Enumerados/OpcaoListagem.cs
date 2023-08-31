using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum OpcaoListagem : long
    {
        [Display(Name = "Outros")]
        Outros = -1,
        [Display(Name = "Todos")]
        Todos = -99
    }
}
