using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class ParecerPropostaMovimentacaoDTO
    {
        [Required(ErrorMessage = "É necessário informar a justificativa da proposta")]
        [MaxLength(150, ErrorMessage = "A justificativa da proposta não pode conter mais que 1000 caracteres")]
        public string Justificativa { get; set; }
    }
}