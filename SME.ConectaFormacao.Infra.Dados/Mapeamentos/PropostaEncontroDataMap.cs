using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaEncontroDataMap : BaseMapAuditavel<PropostaEncontroData>
    {
        public PropostaEncontroDataMap()
        {
            ToTable("proposta_encontro_data");

            Map(t => t.PropostaEncontroId).ToColumn("proposta_encontro_id");
            Map(t => t.DataInicio).ToColumn("data_inicio");
            Map(t => t.DataFim).ToColumn("data_fim");
        }
    }
}
