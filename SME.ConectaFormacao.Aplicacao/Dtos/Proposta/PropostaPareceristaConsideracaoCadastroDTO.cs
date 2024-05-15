using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPareceristaConsideracaoCadastroDTO : PropostaParecerFiltroDTO
    {
        public long? Id { get; set; }
        
        [Required(ErrorMessage = "É necessário informar a descrição da consideração do parecerista da proposta")]
        public string Descricao { get; set; }
    }
}
