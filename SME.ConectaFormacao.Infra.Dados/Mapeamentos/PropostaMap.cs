﻿using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaMap : BaseMapAuditavel<Proposta>
    {
        public PropostaMap()
        {
            ToTable("proposta");

            Map(c => c.AreaPromotoraId).ToColumn("area_promotora_id");
            Map(c => c.TipoFormacao).ToColumn("tipo_formacao");
            Map(c => c.Formato).ToColumn("formato");
            Map(c => c.TipoInscricao).ToColumn("tipo_inscricao");
            Map(c => c.NomeFormacao).ToColumn("nome_formacao");
            Map(c => c.QuantidadeTurmas).ToColumn("quantidade_turmas");
            Map(c => c.QuantidadeVagasTurma).ToColumn("quantidade_vagas_turma");
            Map(c => c.FuncaoEspecificaOutros).ToColumn("funcao_especifica_outros");
            Map(c => c.CriterioValidacaoInscricaoOutros).ToColumn("criterio_validacao_inscricao_outros");
            Map(c => c.ArquivoImagemDivulgacaoId).ToColumn("arquivo_imagem_divulgacao_id");
            Map(c => c.Situacao).ToColumn("situacao");
            Map(c => c.DataInscricaoInicio).ToColumn("data_inscricao_inicio");
            Map(c => c.DataInscricaoFim).ToColumn("data_inscricao_fim");
            Map(c => c.DataRealizacaoInicio).ToColumn("data_realizacao_inicio");
            Map(c => c.DataRealizacaoFim).ToColumn("data_realizacao_fim");
            Map(c => c.CargaHorariaPresencial).ToColumn("carga_horaria_presencial");
            Map(c => c.CargaHorariaSincrona).ToColumn("carga_horaria_sincrona");
            Map(c => c.CargaHorariaDistancia).ToColumn("carga_horaria_distancia");
            Map(c => c.Justificativa).ToColumn("justificativa");
            Map(c => c.Objetivos).ToColumn("objetivos");
            Map(c => c.ConteudoProgramatico).ToColumn("conteudo_programatico");
            Map(c => c.ProcedimentoMetadologico).ToColumn("procedimento_metodologico");
            Map(c => c.Referencia).ToColumn("referencia");
            Map(t => t.CursoComCertificado).ToColumn("curso_com_certificado");
            Map(t => t.AcaoInformativa).ToColumn("acao_informativa");
            Map(t => t.AcaoFormativaTexto).ToColumn("acao_formativa_texto");
            Map(t => t.AcaoFormativaLink).ToColumn("acao_formativa_link");
            Map(t => t.DescricaoDaAtividade).ToColumn("descricao_atividade");
            Map(t => t.FormacaoHomologada).ToColumn("formacao_homologada");
        }
    }
}
