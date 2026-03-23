using Dapper.FluentMap.Dommel.Mapping;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class UeMap : DommelEntityMap<Ue>
    {
        public UeMap()
        {
            ToTable("ue");
            Map(c => c.Id).ToColumn("id").IsIdentity().IsKey();
            Map(c => c.DreId).ToColumn("dre_id");
            Map(c => c.CodigoUe).ToColumn("codigo_ue");
            Map(c => c.NomeEscola).ToColumn("nome_escola");
            Map(c => c.TipoEscola).ToColumn("tipo_escola");
            Map(c => c.SiglaTipoEscola).ToColumn("sigla_tipo_escola");
            Map(c => c.DataAtualizacao).ToColumn("data_atualizacao");
        }
    }
}
