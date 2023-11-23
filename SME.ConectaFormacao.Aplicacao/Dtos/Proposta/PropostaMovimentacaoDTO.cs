using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaMovimentacaoDTO : ParecerPropostaMovimentacaoDTO
    {
        [Required(ErrorMessage = "É necessário informar a situação da movimentação da proposta")]
        public SituacaoProposta Situacao { get; set; }
    }
}