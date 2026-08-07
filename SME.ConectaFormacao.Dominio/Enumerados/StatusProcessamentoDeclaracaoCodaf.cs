using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum StatusProcessamentoDeclaracaoCodaf
    {
        [Display(Name = "Pendente")]
        Pendente = 1,
        [Display(Name = "Em Processamento")]
        EmProcessamento = 2,
        [Display(Name = "Processado com Sucesso")]
        ProcessadoComSucesso = 3,
        [Display(Name = "Processado com Erro")]
        ProcessadoComErro = 4
    }
}
