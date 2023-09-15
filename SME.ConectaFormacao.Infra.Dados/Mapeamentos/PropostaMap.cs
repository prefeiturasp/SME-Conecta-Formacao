using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaMap : BaseMapAuditavel<Proposta>
    {
        public PropostaMap()
        {
            ToTable("proposta");

            Map(c => c.AreaPromotoraId).ToColumn("area_promotora_id");
            Map(c => c.TipoFormacao).ToColumn("tipo_formacao");
            Map(c => c.Modalidade).ToColumn("modalidade");
            Map(c => c.TipoInscricao).ToColumn("tipo_inscricao");
            Map(c => c.NomeFormacao).ToColumn("nome_formacao");
            Map(c => c.QuantidadeTurmas).ToColumn("quantidade_turmas");
            Map(c => c.QuantidadeVagasTurma).ToColumn("quantidade_vagas_turma");
            Map(c => c.FuncaoEspecificaOutros).ToColumn("funcao_especifica_outros");
            Map(c => c.CriterioValidacaoInscricaoOutros).ToColumn("criterio_validacao_inscricao_outros");
            Map(c => c.ArquivoImagemDivulgacaoId).ToColumn("arquivo_imagem_divulgacao_id");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
