using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum FormacaoHomologada
    {
        [Display(Name = "Sim")]
        Sim = 1,
        [Display(Name = "Não (Cursos por IN)")]
        NaoCursosPorIN = 2,
        [Display(Name = "Não (Cursos extras)")]
        NaoCursosExtras = 3
    }
}
