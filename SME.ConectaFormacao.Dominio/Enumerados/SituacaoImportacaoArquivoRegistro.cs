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
        Erro = 4
    }
    
    public static class SituacaoImportacaoArquivoRegistroExtensao
    {
        public static bool PreValidacao(this SituacaoImportacaoArquivoRegistro situacaoImportacaoArquivoRegistro)
        {
            return situacaoImportacaoArquivoRegistro == SituacaoImportacaoArquivoRegistro.CarregamentoInicial;
        }
        
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