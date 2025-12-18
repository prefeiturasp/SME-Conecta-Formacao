namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafInscricao : EntidadeBaseAuditavel
    {
        public int CodafListaPresencaId { get; set; }
        public CodafListaPresenca? CodafListaPresenca { get; set; }
        public long InscricaoId { get; set; }
        public Inscricao? Inscricao { get; set; }
        public decimal PercentualFrequencia { get; set; }
        public bool AtividadeObrigatorio { get; set; }
        public required string ConceitoFinal { get; set; }
        public bool Aprovado { get; set; }
    }
}