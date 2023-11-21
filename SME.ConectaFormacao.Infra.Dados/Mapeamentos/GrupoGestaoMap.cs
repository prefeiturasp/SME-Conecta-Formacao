using Dapper.FluentMap.Dommel.Mapping;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class GrupoGestaoMap : BaseMap<GrupoGestao>
    {
        public GrupoGestaoMap()
        {
            ToTable("grupo_gestao");
            Map(c => c.GrupoId).ToColumn("grupo_id");
            Map(c => c.Nome).ToColumn("nome");
        }
    }
}