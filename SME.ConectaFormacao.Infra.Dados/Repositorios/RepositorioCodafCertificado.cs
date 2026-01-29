using Dapper;
using Npgsql;
using NpgsqlTypes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafCertificado(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafCertificado>(contexto, conexao),
        IRepositorioCodafCertificado
    {
        public async Task<IEnumerable<DadosEmissaoCertificadoCodafDto>> ObterDadosParaEmissaoCertificadosCodafAsync(long codafListaPresencaId)
        {
            const string sql = """
                SELECT 
                	   CILP.ID AS idReferencia,
                       U.NOME AS nomeCompleto,
                       U.LOGIN AS documento,
                       (U.LOGIN <> U.CPF) AS temRf,
                       1 AS tipoParticipacao, -- Cursista
                       P.NOME_FORMACAO AS nomeFormacao,
                       P.DATA_REALIZACAO_INICIO AS dataRealizacao,
                       CILP.CONCEITO_FINAL AS conceitoFinal,
                       CILP.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
                       P.HORAS_TOTAIS AS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA AS cargaHorariaTotalOutra,
                       U.EMAIL AS emailUsuario,
                       CLP.NUMERO_COMUNICADO AS numeroComunicado,
                       CLP.DATA_PUBLICACAO AS dataPublicacao,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                       INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                       INNER JOIN PUBLIC.INSCRICAO AS I  ON CILP.INSCRICAO_ID = I.ID 
                       INNER JOIN PUBLIC.USUARIO AS U  ON I.USUARIO_ID = U.ID
                WHERE  NOT CLP.EXCLUIDO 
                  AND  CILP.APROVADO 
                  AND  NOT CILP.EXCLUIDO
                  AND  CLP.ID = @idCodaf
                  AND  P.CURSO_COM_CERTIFICADO
                UNION ALL
                SELECT 
                       PRT.ID AS idReferencia,
                       PR.NOME_REGENTE AS nomeCompleto,
                       PR.REGISTRO_FUNCIONAL AS documento,
                       TRUE AS temRf, -- Regente sempre tem RF
                       2 AS tipoParticipacao, -- Regente
                       P.NOME_FORMACAO AS nomeFormacao,
                       P.DATA_REALIZACAO_INICIO AS dataRealizacao,
                       NULL AS conceitoFinal,
                       NULL AS percentualFrequencia,
                       P.HORAS_TOTAIS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA cargaHorariaTotalOutra,
                       U.EMAIL AS emailUsuario,
                       CLP.NUMERO_COMUNICADO AS numeroComunicado,
                       CLP.DATA_PUBLICACAO AS dataPublicacao,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao
                FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                       LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
                WHERE  NOT CLP.EXCLUIDO 
                  AND  NOT PRT.EXCLUIDO 
                  AND  NOT PR.EXCLUIDO
                  AND  CLP.ID = @idCodaf
                  AND  P.CURSO_COM_CERTIFICADO
                """;

            return await conexao.Obter().QueryAsync<DadosEmissaoCertificadoCodafDto>(sql, new { idCodaf = codafListaPresencaId });
        }

        public async Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados)
        {
            if (certificados is null || !certificados.Any())
                return;

            const string copyCommand = """
                COPY public.codaf_certificados (
                    codaf_inscricao_lista_presenca_id,
                    proposta_regente_turma_id,
                    tipo_participacao,
                    data_emissao,
                    html_content_snapshot,
                    metadados_json,
                    criado_em,
                    criado_por,
                    criado_login,
                    excluido
                ) FROM STDIN (FORMAT BINARY)
                """;

            using var writer = await ((NpgsqlConnection) conexao.Obter()).BeginBinaryImportAsync(copyCommand);
            foreach (var cert in certificados)
            {
                await writer.StartRowAsync();

                if (cert.CodafInscricaoListaPresencaId.HasValue)
                    await writer.WriteAsync(cert.CodafInscricaoListaPresencaId.Value, NpgsqlDbType.Bigint);
                else
                    await writer.WriteNullAsync();

                if (cert.PropostaRegenteTurmaId.HasValue)
                    await writer.WriteAsync(cert.PropostaRegenteTurmaId.Value, NpgsqlDbType.Bigint);
                else
                    await writer.WriteNullAsync();

                await writer.WriteAsync((int)cert.TipoParticipacao, NpgsqlDbType.Integer);
                await writer.WriteAsync(cert.DataEmissao, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(cert.HtmlContentSnapshot, NpgsqlDbType.Text);

                if (!string.IsNullOrEmpty(cert.MetadadosJson))
                    await writer.WriteAsync(cert.MetadadosJson, NpgsqlDbType.Jsonb);
                else
                    await writer.WriteNullAsync();

                await writer.WriteAsync(DateTimeExtension.HorarioBrasilia(), NpgsqlDbType.Timestamp);
                await writer.WriteAsync(contexto.NomeUsuario, NpgsqlDbType.Varchar);
                await writer.WriteAsync(contexto.UsuarioLogado, NpgsqlDbType.Varchar);
                await writer.WriteAsync(false, NpgsqlDbType.Boolean);
            }
            await writer.CompleteAsync();
        }

        public async Task<IEnumerable<DadosProcessamentoCertificadoCodafDto>> ObterCertificadosParaProcessamentoAsync()
        {
            const string sql = """
                WITH batch_para_processar AS (
                    SELECT id
                    FROM   PUBLIC.CODAF_CERTIFICADOS AS CC
                    WHERE  NOT CC.EXCLUIDO 
                       AND CC.STATUS_PROCESSAMENTO = @statusPendente
                    ORDER  BY id ASC
                    LIMIT  @tamanhoLote
                    FOR    UPDATE SKIP LOCKED
                ),
                certificados_atualizados AS (
                    UPDATE PUBLIC.CODAF_CERTIFICADOS C
                    SET
                        STATUS_PROCESSAMENTO = @statusProcessando,
                        ALTERADO_EM = NOW(),
                        ALTERADO_POR = 'WORKER'
                    FROM batch_para_processar B
                    WHERE C.id = B.id
                    -- Retornamos tudo que precisamos para fazer o JOIN abaixo
                    RETURNING C.ID, 
                              C.CODIGO_CERTIFICADO, 
                              C.HTML_CONTENT_SNAPSHOT,
                              C.CODAF_INSCRICAO_LISTA_PRESENCA_ID, -- FK necessária para o join
                              C.PROPOSTA_REGENTE_TURMA_ID          -- FK necessária para o join
                )
                SELECT 
                   	   CA.ID,
                	   CA.CODIGO_CERTIFICADO AS codigoCertificado,
                	   CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
                       U.NOME AS nomeCompleto,
                       (U.LOGIN <> U.CPF) AS temRf,
                       1 AS tipoParticipacao, -- Cursista
                       P.NOME_FORMACAO AS nomeFormacao,
                       U.EMAIL AS emailUsuario       
                FROM   certificados_atualizados CA
                	   INNER JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA CILP ON CA.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID
                	   INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA CLP ON CILP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                	   INNER JOIN PUBLIC.PROPOSTA_TURMA PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                	   INNER JOIN PUBLIC.PROPOSTA P ON PT.PROPOSTA_ID = P.ID
                       INNER JOIN PUBLIC.INSCRICAO AS I  ON CILP.INSCRICAO_ID = I.ID 
                       INNER JOIN PUBLIC.USUARIO AS U  ON I.USUARIO_ID = U.ID
                UNION ALL
                SELECT        
                   	   CA.ID,
                	   CA.CODIGO_CERTIFICADO AS codigoCertificado,
                	   CA.HTML_CONTENT_SNAPSHOT AS htmlContentSnapshot,
                	   PR.NOME_REGENTE AS nomeCompleto,
                       TRUE AS temRf, -- Regente sempre tem RF
                       2 AS tipoParticipacao, -- Regente
                       P.NOME_FORMACAO AS nomeFormacao,
                       U.EMAIL AS emailUsuario
                FROM   certificados_atualizados CA
                	   INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CA.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR  ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PRT.TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
                       LEFT JOIN PUBLIC.USUARIO AS U ON U.CPF = PR.REGISTRO_FUNCIONAL OR U.LOGIN = PR.REGISTRO_FUNCIONAL
                """;

            return await conexao.Obter().QueryAsync<DadosProcessamentoCertificadoCodafDto>(sql, new
            {
                statusPendente = (int)StatusProcessamentoCertificadoCodaf.Pendente,
                statusProcessando = (int)StatusProcessamentoCertificadoCodaf.EmProcessamento,
                tamanhoLote = 10
            });
        }

        public async Task AtualizarStatusProcessamentoAsync(long id, StatusProcessamentoCertificadoCodaf statusProcessamento, string? chaveObjetoArmazenamento, string? erroProcessamento)
        {
            const string sql = """
                UPDATE PUBLIC.CODAF_CERTIFICADOS
                SET
                    STATUS_PROCESSAMENTO = @statusProcessamento,
                    CHAVE_OBJETO_ARMAZENAMENTO = @chaveObjetoArmazenamento,
                    ERRO_PROCESSAMENTO = @erroProcessamento,
                    ALTERADO_EM = NOW(),
                    ALTERADO_POR = 'WORKER'
                WHERE ID = @id;
                """;
            await conexao.Obter().ExecuteAsync(sql, new
            {
                id,
                statusProcessamento = (int)statusProcessamento,
                chaveObjetoArmazenamento,
                erroProcessamento
            });
        }

        public async Task RecuperarCertificadosTravadosAsync()
        {
            const string sql = """
                UPDATE PUBLIC.CODAF_CERTIFICADOS
                SET
                    STATUS_PROCESSAMENTO = CASE
                        WHEN tentativas_processamento < 3 THEN @statusPendente
                        ELSE @statusErro
                    END,
                    TENTATIVAS_PROCESSAMENTO = TENTATIVAS_PROCESSAMENTO + 1,
                    ERRO_PROCESSAMENTO = CASE
                        WHEN tentativas_processamento < 3 THEN NULL
                        ELSE 'Erro ao processar certificado'
                    END,
                    ALTERADO_EM = NOW(),
                    ALTERADO_POR = 'WORKER-RESILIENCIA'
                WHERE STATUS_PROCESSAMENTO = @statusProcessando
                  AND ALTERADO_EM < (NOW() - INTERVAL '30 minutes'); -- Mas faz tempo demais, uai!;
                """;
            await conexao.Obter().ExecuteAsync(sql, new
            {
                statusPendente = (int)StatusProcessamentoCertificadoCodaf.Pendente,
                statusProcessando = (int)StatusProcessamentoCertificadoCodaf.EmProcessamento,
                statusErro = (int)StatusProcessamentoCertificadoCodaf.ProcessadoComErro
            });
        }
    }
}