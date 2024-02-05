using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTipoInscricao : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public TipoInscricao TipoInscricao { get; set; }
    }
}
