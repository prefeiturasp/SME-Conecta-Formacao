using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum AreaPromotoraTipo
    {
        [Display(Description = "Rede Direta")]
        RedeDireta = 1,
        [Display(Description = "Rede Parceira")]
        RedeParceira = 2
    }
}
