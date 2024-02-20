using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoArquivo
    {
        [Display(Name = "temp")]
        Temp = 1,
        [Display(Name = "imagem divulgacao proposta")]
        ImagemDivulgacaoProposta = 2
    }
}
