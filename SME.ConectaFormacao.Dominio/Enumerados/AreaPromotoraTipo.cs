using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum AreaPromotoraTipo
    {
        [Display(Name = "Rede Direta")]
        RedeDireta = 1,
        [Display(Name = "Rede Parceria")]
        RedeParceria = 2
    }
}
