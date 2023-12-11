using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum CargoFuncaoTipo
    {
        [Display(Name = "Cargo")]
        Cargo = 1,
        [Display(Name = "Função")]
        Funcao = 2,
        [Display(Name = "Outros")]
        Outros = 3
    }
}
