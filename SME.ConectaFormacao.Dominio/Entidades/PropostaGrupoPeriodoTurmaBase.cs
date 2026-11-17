namespace SME.ConectaFormacao.Dominio.Entidades
{
    public abstract class PropostaGrupoPeriodoTurmaBase : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public PropostaTurma PropostaTurma { get; set; } = null!;
    }
}
