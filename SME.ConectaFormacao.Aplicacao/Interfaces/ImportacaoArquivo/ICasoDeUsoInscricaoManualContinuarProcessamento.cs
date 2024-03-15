namespace SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo
{
    public interface ICasoDeUsoInscricaoManualContinuarProcessamento
    {
        Task<bool> Executar(long arquivoImportacaoId);
    }
}
