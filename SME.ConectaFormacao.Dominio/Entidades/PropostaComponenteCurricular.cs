namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaComponenteCurricular : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public long ComponenteCurricularId { get; set; }
    }
}
