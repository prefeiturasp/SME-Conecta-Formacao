using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaParecerFiltroDTO
    {
        [Required(ErrorMessage = "É necessário informar o número da proposta")]
        public long PropostaId { get; set; }
        
        [Required(ErrorMessage = "É necessário informar o campo do parecer da proposta")]
        public CampoParecer Campo { get; set; }
    }
}
