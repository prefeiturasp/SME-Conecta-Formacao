using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CoordenadoriaMap : BaseMapAuditavel<Coordenadoria>
    {
        public CoordenadoriaMap()
        {
            ToTable("coordenadoria");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Sigla).ToColumn("sigla");
            Map(c => c.AreasPromotoras).Ignore();
        }
    }
}
