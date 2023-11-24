using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class AtribuicaoPropostaGrupoGestaoDTO : ParecerPropostaMovimentacaoDTO
    {
        [Required(ErrorMessage = "É necessário informar o grupo gestão para atribuição da proposta")]
        public long GrupoGestaoId { get; set; }
    }
}