using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaParecerCadastroDTO : PropostaParecerFiltroDTO
    {
        public long? Id { get; set; }
        
        [Required(ErrorMessage = "É necessário informar a descrição do parecer da proposta")]
        public string Descricao { get; set; }
    }
}
