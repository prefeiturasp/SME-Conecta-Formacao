using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class DreMap : BaseMapAuditavel<Dre>
    {
        public DreMap()
        {
            ToTable("dre");
            Map(c => c.Codigo).ToColumn("dre_id");
            Map(c => c.Abreviacao).ToColumn("abreviacao");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.DataAtualizacao).ToColumn("data_atualizacao");
        }
    }
}