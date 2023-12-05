using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class AnoMap : BaseMapAuditavel<Ano>
    {
        public AnoMap()
        {
            ToTable("ano");

            Map(c => c.CodigoEOL).ToColumn("codigo_eol");
            Map(c => c.Descricao).ToColumn("descricao");
            Map(c => c.CodigoSerieEnsino).ToColumn("codigo_serie_ensino");
            Map(c => c.Modalidade).ToColumn("modalidade");
            Map(c => c.Todos).ToColumn("todos");
            Map(c => c.Ordem).ToColumn("ordem");
            Map(c => c.AnoLetivo).ToColumn("ano_letivo");
        }
    }
}
