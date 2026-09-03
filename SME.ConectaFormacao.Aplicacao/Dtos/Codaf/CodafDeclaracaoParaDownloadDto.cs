namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafDeclaracaoParaDownloadDto
    {
        public long Id { get; set; }
        public long CodigoDeclaracao { get; set; }
        public string UrlDownload { get; set; } = null!;
        public string NomeCompleto { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
    }
}
