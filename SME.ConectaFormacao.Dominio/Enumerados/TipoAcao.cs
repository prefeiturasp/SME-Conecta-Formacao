using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoAcao
    {
        [Display(Description = "Recuperação de senha")]
        RecuperacaoSenha = 1,

        [Display(Description = "Validação de e-mail")]
        ValidacaoEmail = 2,
    }
}