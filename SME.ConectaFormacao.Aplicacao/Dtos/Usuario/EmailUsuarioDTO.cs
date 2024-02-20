using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class EmailUsuarioDTO
    {
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
    }
}
