using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaVagaRemanecenteMap : BaseMapAuditavel<PropostaVagaRemanecente>
    {
        public PropostaVagaRemanecenteMap()
        {
            ToTable("proposta_vaga_remanecente");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.CargoFuncaoId).ToColumn("cargo_funcao_id");
        }
    }
}
