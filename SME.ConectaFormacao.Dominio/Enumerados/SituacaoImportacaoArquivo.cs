using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoImportacaoArquivo
    {
        [Display(Name = "Enviado")]
        Enviado = 1,

        [Display(Name = "Validando")]
        Validando = 2,
        
        [Display(Name = "Validado")]
        Validado = 3,
        
        [Display(Name = "Processando")]
        Processando = 4,
        
        [Display(Name = "Processado")]
        Processado = 5,
        
        [Display(Name = "Cancelado")]
        Cancelado = 6
    }
    
    public static class SituacaoImportacaoArquivoExtensao
    {
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