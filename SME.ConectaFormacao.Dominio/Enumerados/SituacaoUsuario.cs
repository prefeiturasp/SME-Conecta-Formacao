using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoUsuario
    {
        [Display(Name = "Ativo")]
        Ativo = 1,

        [Display(Name = "Aguardando validação do e-mail")]
        AguardandoValidacaoEmail = 2,

        [Display(Name = "Inativo")]
        Inativo = 3
    }

    public static class SituacaoUsuarioExtensao
    {
        public static bool EhInativo(this SituacaoUsuario situacaoUsuario)
        {
            return situacaoUsuario == SituacaoUsuario.Inativo;
        }
    }
}