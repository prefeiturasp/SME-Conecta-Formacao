using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPareceristaConsideracaoCadastroDTO
    {
        public long? Id { get; set; }
        
        [Required(ErrorMessage = "É necessário informar a descrição da consideração do parecerista da proposta")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "É necessário informar o identificador do parecerista da proposta")]
        public long PropostaPareceristaId { get; set; }
        
        [Required(ErrorMessage = "É necessário informar o campo da consideração do parecerista da proposta")]
        public CampoParecer Campo { get; set; }
    }
}
