namespace SME.ConectaFormacao.Aplicacao.DTOS;

public class AlterarSenhaUsuarioDTO
{
    public string SenhaAtual { get; set; }
    public string SenhaNova { get; set; }
    public string ConfirmarSenha { get; set; }
}