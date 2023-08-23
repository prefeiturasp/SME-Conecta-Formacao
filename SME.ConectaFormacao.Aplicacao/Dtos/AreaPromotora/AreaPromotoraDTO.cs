using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora
{
    public class AreaPromotoraDTO
    {
        [Required(ErrorMessage = "É necessário informar o Nome")]
        [MaxLength(50, ErrorMessage = "O Nome não pode conter mais que 50 caracteres")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É necessário informar o Tipo")]
        public AreaPromotoraTipo Tipo { get; set; }
        [EmailAddress(ErrorMessage = "É nescessário informar um email válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "É necessário informar o Perfil")]
        public Guid GrupoId { get; set; }
        public IEnumerable<AreaPromotoraTelefoneDTO> Telefones { get; set; }
    }
}
