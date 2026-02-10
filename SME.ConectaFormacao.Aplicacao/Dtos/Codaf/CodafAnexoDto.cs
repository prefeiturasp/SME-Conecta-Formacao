using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafAnexoDto
    {
        public long Id { get; set; }
        public long CodafListaPresencaId { get; set; }
        public Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Extensao { get; set; }
        public TipoAnexoCodaf TipoAnexoId { get; set; }
        public string UrlDownload { get; set; } = null!;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
    }
}