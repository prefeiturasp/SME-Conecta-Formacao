using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao
{
    public class AutenticacaoRevalidarDTO
    {
        [Required(ErrorMessage = "Informe o token para revalidar")]
        public string Token { get; set; }
    }
}
