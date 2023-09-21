namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontroTurma : EntidadeBaseAuditavel
    {
        public long PropostaEncontroId { get; set; }
        public short Turma { get; set; }
    }
}
