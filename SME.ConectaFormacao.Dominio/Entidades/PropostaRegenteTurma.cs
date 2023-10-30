namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaRegenteTurma : EntidadeBaseAuditavel
    {
        public long PropostaRegenteId { get; set; }
        public int Turma { get; set; }
    }
}