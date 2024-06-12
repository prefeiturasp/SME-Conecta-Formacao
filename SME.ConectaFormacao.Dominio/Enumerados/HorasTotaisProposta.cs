using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio
{
    public enum HorasTotaisProposta
    {
        [Display(Name = "8h")]
        OitoHoras = 8,
        [Display(Name = "16h")]
        DezesseisHoras = 16,
        [Display(Name = "20h")]
        VinteHoras = 20,
        [Display(Name = "24h")]
        VinteQuatroHoras = 24,
        [Display(Name = "30h")]
        TrintaHoras = 30,
        [Display(Name = "Outra")]
        Outra = 99
    }
}