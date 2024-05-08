using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaPareceristaConsideracao : EntidadeBaseAuditavel
    {
        public long PropostaPareceristaId { get; set; }
        public CampoParecer Campo { get; set; }
        public string Descricao { get; set; }
    }
}
