namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafConferenciaInscricao : EntidadeBaseAuditavel
    {
        public long CodafConferenciaId { get; set; }
        public CodafConferencia? CodafConferencia { get; set; }
        public long InscricaoId { get; set; }
        public Inscricao? Inscricao { get; set; }
        public bool? Participou { get; set; }
    }
}
