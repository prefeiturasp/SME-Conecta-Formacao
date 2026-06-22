namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementarInscricao : EntidadeBaseAuditavel
    {
        public long CodafSuplementarId { get; set; }
        public CodafSuplementar? CodafSuplementar { get; set; }
        public long InscricaoId { get; set; }
        public Inscricao? Inscricao { get; set; }
        public decimal? PercentualFrequencia { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? Aprovado { get; set; }
    }
}