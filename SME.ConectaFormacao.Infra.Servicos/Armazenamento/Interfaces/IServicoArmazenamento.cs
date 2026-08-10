using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces
{
    public interface IServicoArmazenamento
    {
        Task<string> ArmazenarTemporaria(string nomeArquivo, Stream stream, string contentType);
        Task<string> Armazenar(string nomeArquivo, Stream stream, string contentType);
        Task<string> Mover(string nomeArquivo);
        Task<bool> Excluir(string nomeArquivo, string nomeBucket = "");
        Task<IEnumerable<string>> ObterBuckets();
        Task<string> Obter(string nomeArquivo, bool ehPastaTemp);
        Task<Guid> ArmazenarTemporariaGuid(Stream stream, string contentType);
        Task<Guid> MoverGuid(Guid arquivoId);
        string ObterUrlPorGuid(Guid arquivoId, bool ehPastaTemp = false);
        Task<string> ObterUrlPorChaveObjetoAsync(string chaveObjeto, bool ehPastaTemp = false);
        Task<string> UploadCodafAsync(string nomeArquivo, byte[] conteudoPdf);
        Task<Resultado<Stream>> ObterArquivoPorChaveAsync(string chaveObjeto, CancellationToken cancellationToken = default);
    }
}