namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaCriterioCertificacao : EntidadeBaseAuditavel
    {
        public long CriterioCertificacaoId { get; set; }
        public long PropostaId { get; set; }
    }
}