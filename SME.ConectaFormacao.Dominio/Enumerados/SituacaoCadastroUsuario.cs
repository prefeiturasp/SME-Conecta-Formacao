using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoCadastroUsuario
    {
        [Display(Name = "Ativo")]
        Ativo = 1,

        [Display(Name = "Aguardando validação do e-mail")]
        AguardandoValidacaoEmail = 2,
    }
}