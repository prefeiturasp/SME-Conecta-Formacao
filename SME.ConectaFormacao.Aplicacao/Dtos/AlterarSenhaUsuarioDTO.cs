using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.DTOS;

public class AlterarSenhaUsuarioDTO
{
    [Required(ErrorMessage = "É necessário informar a senha atual.")]
    public string SenhaAtual { get; set; }
    [Required(ErrorMessage = "É necessário informar a nova senha.")]
    [MinLength(8, ErrorMessage = "A senha deve conter no mínimo 8 caracteres")]
    [MaxLength(12, ErrorMessage = "A senha deve conter no máximo 12 caracteres")]
    public string SenhaNova { get; set; }
    [Required(ErrorMessage = "É necessário informar a confirmação de senha.")]
    [MinLength(8, ErrorMessage = "A senha deve conter no mínimo 8 caracteres")]
    [MaxLength(12, ErrorMessage = "A senha deve conter no máximo 12 caracteres")]
    public string ConfirmarSenha { get; set; }
}