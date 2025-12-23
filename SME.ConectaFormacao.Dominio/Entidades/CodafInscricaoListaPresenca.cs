namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafInscricaoListaPresenca : EntidadeBaseAuditavel
    {
        public long CodafListaPresencaId { get; set; }
        public CodafListaPresenca? CodafListaPresenca { get; set; }
        public long InscricaoId { get; set; }
        public Inscricao? Inscricao { get; set; }
        public decimal? PercentualFrequencia { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? Aprovado { get; set; }
    }
}