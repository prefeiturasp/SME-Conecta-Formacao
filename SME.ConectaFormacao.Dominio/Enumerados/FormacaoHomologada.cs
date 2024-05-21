using SME.ConectaFormacao.Dominio.Extensoes;
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

    public static class FormacaoHomologadaExtensao
    {
        public static bool EstaHomologada(this FormacaoHomologada? valor)
        {
            if (valor.EhNulo())
                return false;

            return valor == FormacaoHomologada.Sim;
        }

        public static bool NaoEstaHomologada(this FormacaoHomologada? valor)
        {
            return !EstaHomologada(valor);
        }
    }
}
