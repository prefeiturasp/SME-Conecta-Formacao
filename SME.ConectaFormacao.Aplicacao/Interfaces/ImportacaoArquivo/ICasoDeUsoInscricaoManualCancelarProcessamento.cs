namespace SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo
{
    public interface ICasoDeUsoInscricaoManualCancelarProcessamento
    {
        Task<bool> Executar(long arquivoImportacaoId);
    }
}
