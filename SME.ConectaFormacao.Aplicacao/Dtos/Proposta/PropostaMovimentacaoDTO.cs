using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaMovimentacaoDTO
    {
        [Required(ErrorMessage = "É necessário informar o parecer da proposta")]
        [MaxLength(150, ErrorMessage = "O parecer da proposta não pode conter mais que 1000 caracteres")]
        public string Parecer { get; set; }
        
        [Required(ErrorMessage = "É necessário informar a situação da movimentação da proposta")]
        public SituacaoProposta Situacao { get; set; }
    }
}