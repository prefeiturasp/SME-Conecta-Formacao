using Dapper.FluentMap.Dommel.Mapping;
using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class BaseMap<T> : DommelEntityMap<T> where T : EntidadeBase
    {
        public BaseMap()
        {
            Map(c => c.Id).ToColumn("id").IsIdentity().IsKey();
        }
    }
}
