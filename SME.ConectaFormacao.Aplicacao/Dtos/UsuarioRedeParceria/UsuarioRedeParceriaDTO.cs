using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria
{
    public class UsuarioRedeParceriaDTO
    {
        public long AreaPromotoraId { get; set; }
        [Required(ErrorMessage = "É necessário informar o Nome")]
        public string Nome { get; set; } = null!;
        public string? NomeSocial { get; set; }
        [Required(ErrorMessage = "É necessário informar o Documento")]
        public string Cpf { get; set; } = null!;
        [Required(ErrorMessage = "É necessário informar o E-mail")]
        [EmailAddress(ErrorMessage = "O E-mail informado é inválido")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "É necessário informar o Telefone")]
        public string Telefone { get; set; } = null!;
        [Required(ErrorMessage = "É necessário informar a Situação")]
        public SituacaoUsuario Situacao { get; set; }
    }
}
