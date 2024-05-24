using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaParecerFiltroDTO
    {
        [Required(ErrorMessage = "É necessário informar o número da proposta")]
        public long PropostaId { get; set; }

        [Required(ErrorMessage = "É necessário informar o campo do parecer da proposta")]
        public CampoConsideracao Campo { get; set; }
    }
}
