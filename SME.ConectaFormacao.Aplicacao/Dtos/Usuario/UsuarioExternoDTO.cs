using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class UsuarioExternoDTO
    {
        public string? Login { get; set; }

        [Required(ErrorMessage = "É necessário informar a Unidade.")]
        public string CodigoUnidade { get; set; }

        [Required(ErrorMessage = "É necessário informar o e-mail.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "É necessário informar o e-mail @edu")]
        public string EmailEducacional { get; set; }
        
        public TipoUsuario? Tipo { get; set; }

        [Required(ErrorMessage = "É necessário informar o nome.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É necessário informar o cpf.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "É necessário informar a senha.")]
        public string Senha { get; set; }


        [Required(ErrorMessage = "É necessário informar confirmar senha.")]
        public string ConfirmarSenha { get; set; }

        public DateTime? CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }

    }
}