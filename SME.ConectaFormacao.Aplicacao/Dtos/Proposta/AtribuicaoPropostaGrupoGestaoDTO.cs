using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class AtribuicaoPropostaGrupoGestaoDTO
    {
        [Required(ErrorMessage = "É necessário informar o grupo gestão para atribuição da proposta")]
        public long GrupoGestaoId { get; set; }
    }
}