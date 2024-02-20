using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaPublicoAlvoMap : BaseMapAuditavel<PropostaPublicoAlvo>
    {
        public PropostaPublicoAlvoMap()
        {
            ToTable("proposta_publico_alvo");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.CargoFuncaoId).ToColumn("cargo_funcao_id");
        }
    }
}
