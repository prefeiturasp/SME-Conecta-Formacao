namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaRegenteTurma : EntidadeBaseAuditavel
    {
        public long PropostaRegenteId { get; set; }
        public long TurmaId { get; set; }

        public PropostaTurma Turma { get; set; }
    }
}