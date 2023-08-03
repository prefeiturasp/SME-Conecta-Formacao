using Dapper.FluentMap.Dommel.Mapping;
using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class BaseMapAuditavel<T> : BaseMap<T> where T : EntidadeBaseAuditavel
    {
        public BaseMapAuditavel()
        {
            Map(c => c.CriadoEm).ToColumn("criado_em");
            Map(c => c.CriadoPor).ToColumn("criado_por");
            Map(c => c.AlteradoEm).ToColumn("alterado_em");
            Map(c => c.AlteradoPor).ToColumn("alterado_por");
            Map(c => c.AlteradoLogin).ToColumn("alterado_login");
            Map(c => c.CriadoLogin).ToColumn("criado_login");
        }
    }
}
