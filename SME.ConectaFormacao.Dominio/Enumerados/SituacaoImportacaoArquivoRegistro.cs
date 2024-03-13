using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoImportacaoArquivoRegistro
    {
        [Display(Name = "Validado")]
        Validado = 1,
        
        [Display(Name = "Processado")]
        Processado = 2,
        
        [Display(Name = "Erro")]
        Erro = 3
    }
    
    public static class SituacaoImportacaoArquivoRegistroExtensao
    {
        public static bool Validado(this SituacaoImportacaoArquivoRegistro situacaoImportacaoArquivoRegistro)
        {
            return situacaoImportacaoArquivoRegistro == SituacaoImportacaoArquivoRegistro.Validado;
        }
        
        public static bool Processando(this SituacaoImportacaoArquivoRegistro situacaoImportacaoArquivoRegistro)
        {
            return situacaoImportacaoArquivoRegistro == SituacaoImportacaoArquivoRegistro.Processado;
        }
        
        public static bool Erro(this SituacaoImportacaoArquivoRegistro situacaoImportacaoArquivoRegistro)
        {
            return situacaoImportacaoArquivoRegistro == SituacaoImportacaoArquivoRegistro.Erro;
        }
    }
}