namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTurmaVaga : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long? InscricaoId { get; set; }
    }
}
