namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaModalidadeAnoTurmaEtapa : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public long AnoTurmaId { get; set; }
        public int Modalidade { get; set; }
        public int EtapaEja { get; set; }
    }
}
