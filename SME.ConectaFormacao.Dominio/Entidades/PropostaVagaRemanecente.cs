namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaVagaRemanecente : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public long CargoFuncaoId { get; set; }
    }
}
