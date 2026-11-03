namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodoInscricaoTurma : EntidadeBaseAuditavel
    {
        public long GrupoPeriodoInscricaoId { get; set; }
        public long PropostaTurmaId { get; set; }
        public PropostaGrupoPeriodoInscricao GrupoPeriodoInscricao { get; set; } = null!;
        public PropostaTurma PropostaTurma { get; set; } = null!;
    }
}
