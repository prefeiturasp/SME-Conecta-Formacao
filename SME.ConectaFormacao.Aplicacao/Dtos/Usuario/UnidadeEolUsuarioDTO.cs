using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class UnidadeEolUsuarioDTO
    {
        [Required(ErrorMessage = "O Código da unidade do EOL é obrigatório")]
        public string CodigoEolUnidade { get; set; }
    }
}
