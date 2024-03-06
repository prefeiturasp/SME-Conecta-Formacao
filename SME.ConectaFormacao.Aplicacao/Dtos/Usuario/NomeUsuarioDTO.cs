using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class NomeUsuarioDTO
    {
        [Required(ErrorMessage = "É necessário informar o nome")]
        public string Nome { get; set; }
    }
}
