namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTutorTurma : EntidadeBaseAuditavel
    {
        public long PropostaTutorId { get; set; }
        public long TurmaId { get; set; }

        public PropostaTurma Turma { get; set; }
    }
}