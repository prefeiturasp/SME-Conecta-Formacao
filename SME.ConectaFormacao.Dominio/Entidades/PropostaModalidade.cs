using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaModalidade : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public Modalidade Modalidade { get; set; }
    }
}
