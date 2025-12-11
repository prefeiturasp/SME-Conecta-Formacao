using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoImportacaoArquivoRegistro
    {
        [Display(Name = "Carregamento inicial")]
        CarregamentoInicial = 1,

        [Display(Name = "Validado")]
        Validado = 2,

        [Display(Name = "Processado")]
        Processado = 3,

        [Display(Name = "Erro")]
        Erro = 4,

        [Display(Name = "Aviso")]
        Aviso = 5
    }
}