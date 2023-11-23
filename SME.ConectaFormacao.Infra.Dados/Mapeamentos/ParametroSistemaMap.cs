

using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class ParametroSistemaMap : BaseMapAuditavel<ParametroSistema>
    {
        public ParametroSistemaMap()
        {
            ToTable("parametro_sistema");
            Map(c => c.Ano).ToColumn("ano");
            Map(c => c.Ativo).ToColumn("ativo");
            Map(c => c.Descricao).ToColumn("descricao");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Valor).ToColumn("valor");
        }
    }
}