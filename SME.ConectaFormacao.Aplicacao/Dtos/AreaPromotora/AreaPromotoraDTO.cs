using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora
{
    public class AreaPromotoraDTO
    {
        [Required(ErrorMessage = "É necessário informar a Área promotora")]
        [MaxLength(50, ErrorMessage = "A Área promotora não pode conter mais que 50 caracteres")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É necessário informar o Tipo")]
        public AreaPromotoraTipo Tipo { get; set; }
        [MaxLength(100, ErrorMessage = "O Email não pode conter mais que 100 caracteres")]
        public string Email { get; set; }
        [Required(ErrorMessage = "É necessário informar o Perfil")]
        public Guid GrupoId { get; set; }
        public IEnumerable<AreaPromotoraTelefoneDTO> Telefones { get; set; }
    }
}
