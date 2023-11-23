using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaMovimentacao : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string Parecer { get; set; }
        public SituacaoProposta Situacao { get; set; }
    }
}