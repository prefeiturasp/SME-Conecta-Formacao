using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoImportacaoArquivo
    {
        [Display(Name = "Carregamento inicial")]
        CarregamentoInicial = 1,
        
        [Display(Name = "Enviado")]
        Enviado = 2,

        [Display(Name = "Validando")]
        Validando = 3,
        
        [Display(Name = "Validado")]
        Validado = 4,
        
        [Display(Name = "Processando")]
        Processando = 5,
        
        [Display(Name = "Processado")]
        Processado = 6,
        
        [Display(Name = "Cancelado")]
        Cancelado = 7
    }
    
    public static class SituacaoImportacaoArquivoExtensao
    {
        public static bool EhCarregamentoInicial(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.CarregamentoInicial;
        }
        
        public static bool Enviado(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Enviado;
        }
        
        public static bool Validando(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Validando;
        }
        
        public static bool Validado(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Validado;
        }
        
        public static bool Processando(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Processando;
        }
        
        public static bool Processado(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Processado;
        }
        
        public static bool Cancelado(this SituacaoImportacaoArquivo situacaoImportacaoArquivo)
        {
            return situacaoImportacaoArquivo == SituacaoImportacaoArquivo.Cancelado;
        }
    }
}