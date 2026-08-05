namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCursoNaoHomologadoAnexo : EntidadeBaseAuditavel
    {
        public long CodafCursoNaoHomologadoId { get; set; }
        public CodafCursoNaoHomologado? CodafCursoNaoHomologado { get; set; }
        public required Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Extensao { get; set; }
    }
}