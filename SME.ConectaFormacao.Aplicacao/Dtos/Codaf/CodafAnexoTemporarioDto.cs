using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafAnexoTemporarioDto
    {
        public Guid ArquivoCodigo { get; set; }
        public string NomeArquivo { get; set; } = null!;
        public string Extensao { get; set; } = null!;
        public string UrlDownload { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long TamanhoBytes { get; set; }
    }
}
