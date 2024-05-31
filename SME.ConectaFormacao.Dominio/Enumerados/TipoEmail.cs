using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoEmail
    {
        [Display(Name = "Funcionário de Unidades Parceiras")]
        FuncionarioUnidadeParceira = 1,

        [Display(Name = "Estagiários")]
        Estagiario = 2,
    }

    public static class TipoEmailExtensao
    {
        public static bool EhFuncionarioUnidadeParceira(this TipoEmail valor)
        {
            return valor == TipoEmail.FuncionarioUnidadeParceira;
        }

        public static bool EhEstagiario(this TipoEmail valor)
        {
            return valor == TipoEmail.Estagiario;
        }
    }
}