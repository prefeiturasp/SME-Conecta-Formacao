namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoInscricaoExtendidoDto
    {
        public long Id { get; set; }
        public long CodafCursoNaoHomologadoId { get; set; }
        public long InscricaoId { get; set; }
        public bool Participou { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public string? Nome { get; set; }
        public string? Login { get; set; }
        public string? Cpf { get; set; }
    }
}

