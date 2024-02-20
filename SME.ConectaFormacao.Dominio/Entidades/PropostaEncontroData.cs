namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontroData : EntidadeBaseAuditavel
    {
        public long PropostaEncontroId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
