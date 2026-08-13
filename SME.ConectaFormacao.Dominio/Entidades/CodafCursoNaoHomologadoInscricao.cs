namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCursoNaoHomologadoInscricao : EntidadeBaseAuditavel
    {
        public long CodafCursoNaoHomologadoId { get; set; }
        public CodafCursoNaoHomologado? CodafCursoNaoHomologado { get; set; }
        public long InscricaoId { get; set; }
        public Inscricao? Inscricao { get; set; }
        public bool Participou { get; set; }
    }
}