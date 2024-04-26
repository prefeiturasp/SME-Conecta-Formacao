using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaParecer : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public CampoParecer Campo { get; set; }
        public string Descricao { get; set; }
        public SituacaoParecer Situacao { get; set; }
    }
}
