using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoImportacaoArquivo
    {
        [Display(Name = "Inscrição manuail")]
        Inscricao_Manual = 1,
    }

    public static class TipoImportacaoArquivoExtensao
    {
        public static bool EhInscricaoManual(this TipoImportacaoArquivo tipoImportacaoArquivo)
        {
            return tipoImportacaoArquivo == TipoImportacaoArquivo.Inscricao_Manual;
        }
    }
}