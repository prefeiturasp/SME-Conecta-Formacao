namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTurma : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string Nome { get; set; }
        public long DreId { get; set; }
    }
}
