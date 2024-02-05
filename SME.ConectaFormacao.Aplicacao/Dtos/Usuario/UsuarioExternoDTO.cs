using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class UsuarioExternoDTO
    {
        public string? Login { get; set; }

        public string? CodigoUe { get; set; }

        [Required(ErrorMessage = "É necessário informar o e-mail.")]
        public string Email { get; set; }
        public TipoUsuario? Tipo { get; set; }

        [Required(ErrorMessage = "É necessário informar o nome.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É necessário informar o cpf.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "É necessário informar a senha.")]
        public string Senha { get; set; }


        [Required(ErrorMessage = "É necessário informar confirmar senha.")]
        public string ConfirmarSenha { get; set; }

    }
}