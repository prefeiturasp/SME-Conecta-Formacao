namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodoTurma : EntidadeBaseAuditavel
    {
        public long GrupoPeriodoId { get; set; }
        public long PropostaTurmaId { get; set; }
        public PropostaGrupoPeriodo GrupoPeriodo { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
    }
}