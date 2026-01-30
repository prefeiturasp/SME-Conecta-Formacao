namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class DadosCertificadoUsuarioParaDownloadDto
    {
        public long Id { get; set; }
        public long CodigoCertificado { get; set; }
        public string? ChaveObjetoArmazenamento { get; set; }
        public string NomeCompleto { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
    }
}
