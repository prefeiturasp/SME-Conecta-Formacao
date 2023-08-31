using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum Modalidade
    {
        [Display(Name = "Presencial")]
        Presencial = 1,
        [Display(Name = "A Distância")]
        Distancia = 2,
        [Display(Name = "Híbrido")]
        Hibrido = 3
    }
}
