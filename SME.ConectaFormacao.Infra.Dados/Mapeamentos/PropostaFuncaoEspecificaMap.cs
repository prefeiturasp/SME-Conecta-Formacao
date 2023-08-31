using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaFuncaoEspecificaMap : BaseMapAuditavel<PropostaFuncaoEspecifica>
    {
        public PropostaFuncaoEspecificaMap()
        {
            ToTable("proposta_funcao_especifica");

            Map(m => m.PropostaId).ToColumn("proposta_id");
            Map(m => m.CargoFuncaoId).ToColumn("cargo_funcao_id");
        }
    }
}
