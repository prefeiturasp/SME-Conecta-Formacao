namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontroTurma : EntidadeBaseAuditavel
    {
        public long PropostaEncontroId { get; set; }
        public long TurmaId { get; set; }

        public PropostaTurma Turma { get; set; }
    }
}
