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

    public static class AreaPromotoraTipoExtensao
    {
        public static bool EhRedeDireta(this AreaPromotoraTipo tipo)
        {
            return tipo == AreaPromotoraTipo.RedeDireta;
        }

        public static bool EhRedeParceria(this AreaPromotoraTipo tipo)
        {
            return tipo == AreaPromotoraTipo.RedeParceria;
        }
    }
}
