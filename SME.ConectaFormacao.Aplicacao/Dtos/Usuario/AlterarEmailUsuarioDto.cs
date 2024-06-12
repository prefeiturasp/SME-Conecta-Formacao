using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class AlterarEmailUsuarioDto
    {
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o Login")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Informe a Senha")]
        public string Senha { get; set; }
    }
}