using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoEmail
    {
        [Display(Name = "Funcionário de Unidades Parceiras")]
        FuncionarioUnidadeParceira = 1,   
        [Display(Name = "Estagiários")]
        Estagiario = 2,   
    }
}