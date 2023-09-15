namespace SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo
{
    public interface ICasoDeUsoArquivoExcluir
    {
        Task<bool> Executar(Guid[] codigos);
    }
}
