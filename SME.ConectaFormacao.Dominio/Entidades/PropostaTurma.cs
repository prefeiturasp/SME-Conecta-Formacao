namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTurma : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string Nome { get; set; }
        public long[] DresIds { get; set; }

        public IEnumerable<PropostaTurmaDre> Dres { get; set; }
        public Proposta Proposta { get; set; }
    }
}
