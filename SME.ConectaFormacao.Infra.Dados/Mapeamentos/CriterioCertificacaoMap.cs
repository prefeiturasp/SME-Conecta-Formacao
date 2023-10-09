using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CriterioCertificacaoMap : BaseMapAuditavel<CriterioCertificacao>
    {
        public CriterioCertificacaoMap()
        {
            ToTable("criterio_certificacao");
            Map(c => c.Descricao).ToColumn("descricao");
        }
    }
}