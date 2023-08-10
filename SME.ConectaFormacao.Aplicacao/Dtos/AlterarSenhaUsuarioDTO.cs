using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.DTOS;

public class AlterarSenhaUsuarioDTO
{
    [Required(ErrorMessage = "É necessário informar a senha atual.")]
    public string SenhaAtual { get; set; }
    [Required(ErrorMessage = "É necessário informar a nova senha.")]
    public string SenhaNova { get; set; }
    [Required(ErrorMessage = "É necessário informar a confirmação de senha.")]
    public string ConfirmarSenha { get; set; }
}