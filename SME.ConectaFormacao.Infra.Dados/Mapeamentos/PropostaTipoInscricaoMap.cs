using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTipoInscricaoMap : BaseMapAuditavel<PropostaTipoInscricao>
    {
        public PropostaTipoInscricaoMap()
        {
            ToTable("Proposta_tipo_inscricao");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.TipoInscricao).ToColumn("tipo_inscricao");
        }
    }
}
