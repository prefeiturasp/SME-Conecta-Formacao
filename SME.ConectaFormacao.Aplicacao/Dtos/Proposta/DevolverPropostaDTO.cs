using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class DevolverPropostaDTO
    {
        [Required(ErrorMessage = "A justificativa deve ser informada.")]
        public string Justificativa { get; set; }
    }
}