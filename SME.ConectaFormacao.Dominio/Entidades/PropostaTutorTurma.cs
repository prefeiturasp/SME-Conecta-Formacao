namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTutorTurma : EntidadeBaseAuditavel
    {
        public long PropostaTutorId { get; set; }
        public int Turma { get; set; }
    }
}