using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafSuplementar(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafSuplementar>(contexto, conexao), IRepositorioCodafSuplementar
    {
        public async Task<ResultadoPaginado<ListagemResultadoCodafSuplementarDto>> ObterListagemResultadoCodafSuplementarPorFiltroAsync(FiltroListagemResultadoCodafSuplementarDto filtro)
        {
            const string sqlBaseJoins = """
                FROM  PUBLIC.CODAF_SUPLEMENTAR AS CS
                INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID 
                INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
                INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID 
                INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON P.AREA_PROMOTORA_ID = AP.ID
                """;
            const string sqlBaseOrderBy = """
                ORDER  BY
                        CS.CRIADO_EM
                """;

            var condicoesWhere = new StringBuilder("WHERE NOT CS.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO ");
            var parametros = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
            {
                condicoesWhere.Append(" AND f_unaccent(P.NOME_FORMACAO) ILIKE f_unaccent(@nomeFormacao) ");
                parametros.Add("nomeFormacao", $"%{filtro.NomeFormacao}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoFormacao))
            {
                condicoesWhere.Append(" AND CAST(P.ID AS TEXT) ILIKE @codigoFormacao ");
                parametros.Add("codigoFormacao", $"{filtro.CodigoFormacao.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.NumeroHomologacao))
            {
                condicoesWhere.Append(" AND CAST(P.NUMERO_HOMOLOGACAO AS TEXT) ILIKE @numeroHomologacao ");
                parametros.Add("numeroHomologacao", $"{filtro.NumeroHomologacao.Trim()}%");
            }

            if (filtro.PropostaTurmaId is not null)
            {
                condicoesWhere.Append(" AND PT.ID = @propostaTurmaId ");
                parametros.Add("propostaTurmaId", filtro.PropostaTurmaId.Value);
            }

            if (filtro.AreaPromotoraId is not null)
            {
                condicoesWhere.Append(" AND AP.ID = @areaPromotoraId ");
                parametros.Add("areaPromotoraId", filtro.AreaPromotoraId.Value);
            }

            if (filtro.Status is not null)
            {
                condicoesWhere.Append(" AND CS.STATUS = @status ");
                parametros.Add("status", filtro.Status.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                SELECT COUNT(1)
                {sqlBaseJoins}
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemResultadoCodafSuplementarDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = 0
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);
            parametros.Add("statusPendente", StatusProcessamentoCertificadoCodaf.Pendente);
            parametros.Add("statusEmProcessamento", StatusProcessamentoCertificadoCodaf.EmProcessamento);
            parametros.Add("statusProcessadoComSucesso", StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso);
            parametros.Add("statusProcessadoComErro", StatusProcessamentoCertificadoCodaf.ProcessadoComErro);

            var sqlConsulta = new StringBuilder($"""
                SELECT CS.ID,
                       P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       p.NOME_FORMACAO AS nomeFormacao,
                       p.ID AS codigoFormacao,
                       pt.NOME AS nomeTurma,
                       ap.NOME AS nomeAreaPromotora,
                       CS.STATUS,
                       CASE 
                	        -- 0: Sem Cetificado
                           WHEN P.CURSO_COM_CERTIFICADO = FALSE THEN 0

                           -- 1: Pendente Emissão
                           WHEN NOT EXISTS (
                               SELECT 1 
                               FROM PUBLIC.CODAF_SUPLEMENTAR_LOG_REMESSA_CONCLUSAO AS L 
                               WHERE L.CODAF_SUPLEMENTAR_ID = CS.ID
                           ) THEN 1

                           -- 3: Em Processamento
                           WHEN EXISTS (
                           	SELECT 1
                           	FROM  PUBLIC.CODAF_CERTIFICADOS AS CC
                           	WHERE CC.CODAF_SUPLEMENTAR_ID = CS.ID AND CC.STATUS_PROCESSAMENTO IN (@statusPendente, @statusEmProcessamento)
                           ) THEN 3

                           -- 4: Emitido
                           WHEN EXISTS (
                           	SELECT 1
                           	FROM  PUBLIC.CODAF_CERTIFICADOS AS CC
                           	WHERE CC.CODAF_SUPLEMENTAR_ID = CS.ID AND CC.STATUS_PROCESSAMENTO IN (@statusProcessadoComSucesso, @statusProcessadoComErro)
                           ) THEN 4

                           -- 2: Disponível para Emissão
                           ELSE 2
                       END AS statusCertificacaoTurma,
                       CLP.CODIGO_CURSO_EOL codigoCursoEol,
                       CLP.CODIGO_NIVEL codigoNivel,
                       CASE 
                       WHEN EXISTS (
                        SELECT 1
                        FROM PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO AS CSI
                        WHERE CSI.CODAF_SUPLEMENTAR_ID = CS.ID AND CSI.APROVADO = TRUE
                        ) THEN TRUE
                        ELSE FALSE
                       END AS possuiAprovacoes
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlBaseOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens = await conn.QueryAsync<ListagemResultadoCodafSuplementarDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemResultadoCodafSuplementarDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<CodafSuplementar?> ObterPorIdDetalhadoAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                -- 1. Dados do Cabeçalho (CODAF + Proposta + Turma)
                {sqlObterCodafPorIdComPropostaEPropostaTurma}

                -- 2. Retificações
                {sqlObterRetificacoesPorIdCodaf}

                -- 3. Anexos
                {sqlObterAnexosPorIdCodaf}

                -- 4. Inscritos (A lista grande)
                {sqlObterInscricoesDaListaPorIdCodaf}

                -- 5. Critérios de Certificação
                {CodafQueries.SqlObterCriteriosCertificacaoPorIdCodaf}

                -- 6. Certificados
                {sqlCertificadosPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafSuplementar = multi.Read<CodafSuplementar, Proposta, PropostaTurma, CodafSuplementar>(
                (cs, p, pt) =>
                {
                    cs.Proposta = p;
                    cs.PropostaTurma = pt;
                    return cs;
                },
            splitOn: "ID,ID").SingleOrDefault();

            if (codafSuplementar is null)
                return null;

            codafSuplementar.CodafRetificacoes = [.. await multi.ReadAsync<CodafSuplementarRetificacao>()];
            codafSuplementar.CodafAnexos = [.. await multi.ReadAsync<CodafSuplementarAnexo>()];
            codafSuplementar.CodafInscricoes = [.. multi.Read<CodafSuplementarInscricao, Usuario, CodafSuplementarInscricao>(
                (csi, usuario) =>
                {
                    csi.Inscricao = new()
                    {
                        Usuario = usuario
                    };
                    return csi;
                },
                splitOn: "LOGIN")];

            if (codafSuplementar.Proposta == null)
                return codafSuplementar;

            codafSuplementar.Proposta.CriterioCertificacao = [.. await multi.ReadAsync<PropostaCriterioCertificacao>()];
            codafSuplementar.CodafCertificados = [.. await multi.ReadAsync<CodafCertificado>()];
            
            return codafSuplementar;
        }

        public override async Task<CodafSuplementar?> ObterNaoExcluidosPorIdAsync(long id)
        {
            var conn = conexao.Obter();
            var sql = $"""
                -- 1. CODAF
                {sqlObterCodafPorId}              

                -- 5. Certificados
                {sqlCertificadosPorIdCodaf}
                """;

            var parametros = new { id };

            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var codafSuplementar = multi.Read<CodafSuplementar>().SingleOrDefault();

            if (codafSuplementar is null)
                return null;
          
            codafSuplementar.CodafCertificados = [.. await multi.ReadAsync<CodafCertificado>()];
            return codafSuplementar;
        }

        public async Task<CodafSuplementar?> ObterPorIdCodafListaPresenca(long idCodafListaPresenca)
        {
            var conn = conexao.Obter();
            var parametros = new { idCodafListaPresenca };
            var codafSuplementar = await conn.QueryAsync<CodafSuplementar>(
                sqlObterCodafSuplementarPorIdCodafListaPresenca,
                parametros);
            return codafSuplementar.FirstOrDefault();
        }

        public async Task ExcluirAsync(long id)
        {
            var conn = conexao.Obter();
            using var transaction = conn.BeginTransaction();

            try
            {
                var parametrosAtualizacao = new
                {
                    Id = id,
                    Excluido = true,
                    AlteradoEm = DateTimeExtension.HorarioBrasilia(),
                    AlteradoPor = contexto.NomeUsuario,
                    AlteradoLogin = contexto.UsuarioLogado
                };

                const string sqlSuplementar = """
                    UPDATE PUBLIC.CODAF_SUPLEMENTAR
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  ID = @Id
                    """;
                await conn.ExecuteAsync(sqlSuplementar, parametrosAtualizacao, transaction);

                const string sqlInscricoes = """
                    UPDATE PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_SUPLEMENTAR_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlInscricoes, parametrosAtualizacao, transaction);

                const string sqlAnexos = """
                    UPDATE PUBLIC.CODAF_SUPLEMENTAR_ANEXO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_SUPLEMENTAR_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlAnexos, parametrosAtualizacao, transaction);

                const string sqlRetificacoes = """
                    UPDATE PUBLIC.CODAF_SUPLEMENTAR_RETIFICACAO
                    SET    EXCLUIDO = @Excluido,
                           ALTERADO_EM = @AlteradoEm,
                           ALTERADO_POR = @AlteradoPor,
                           ALTERADO_LOGIN = @AlteradoLogin
                    WHERE  CODAF_SUPLEMENTAR_ID = @Id and NOT EXCLUIDO
                    """;

                await conn.ExecuteAsync(sqlRetificacoes, parametrosAtualizacao, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<DadosConsultaParaTxtEolDto>?> ObterDadosRemessaConclusaoCodafSuplementarAsync(long id)
        {
            var conn = conexao.Obter();
            const string query = """
                SELECT U.LOGIN registroFuncional,
                       CS.CODIGO_CURSO_EOL codigoCursoEol,
                       P.DATA_REALIZACAO_FIM dataFimCurso,
                       CS.CODIGO_NIVEL codigoNivel,
                       P.NUMERO_HOMOLOGACAO numeroHomologacao,
                       P.HORAS_TOTAIS horasTotais,
                       P.CARGA_HORARIA_TOTAL_OUTRA cargaHorariaTotalOutra,
                       PT.NOME nomeTurma
                FROM   PUBLIC.CODAF_SUPLEMENTAR AS CS
                       INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
                       INNER JOIN PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO AS CSI  ON CSI.CODAF_SUPLEMENTAR_ID = CS.ID
                       INNER JOIN PUBLIC.INSCRICAO AS INSCR ON INSCR.ID = CSI.INSCRICAO_ID
                       INNER JOIN PUBLIC.USUARIO AS U ON U.ID = INSCR.USUARIO_ID
                       INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.ID = CLP.PROPOSTA_TURMA_ID
                       INNER JOIN PUBLIC.PROPOSTA AS P ON P.ID = PT.PROPOSTA_ID
                WHERE NOT CLP.EXCLUIDO AND NOT CSI.EXCLUIDO AND NOT INSCR.EXCLUIDO 
                  AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO 
                  AND CSI.APROVADO
                  AND CS.ID = @id;
                """;
            var parametros = new { id };
            var resultado = await conn.QueryAsync<DadosConsultaParaTxtEolDto>(query, parametros);
            return resultado;
        }

        private const string sqlObterCodafPorId = """
            SELECT CS.ID as id,
                   CLP.ID AS codafId,
                   CS.DATA_PUBLICACAO AS dataPublicacao,
                   CS.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                   CS.NUMERO_COMUNICADO AS numeroComunicado,
                   CS.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                   CS.CODIGO_CURSO_EOL AS codigoCursoEol,
                   CS.CODIGO_NIVEL AS codigoNivel,
                   CS.OBSERVACAO,
                   CS.STATUS,
                   CS.ALTERADO_EM AS alteradoEm,
                   CS.ALTERADO_POR AS alteradoPor,
                   CS.ALTERADO_LOGIN AS alteradoLogin,
                   CS.CRIADO_EM AS criadoEm,
                   CS.CRIADO_POR AS criadoPor,
                   CS.CRIADO_LOGIN AS criadoLogin
            FROM PUBLIC.CODAF_SUPLEMENTAR AS CS   
            INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
            WHERE NOT CS.EXCLUIDO
              AND CS.ID = @id;
            """;

        private const string sqlObterCodafPorIdComPropostaEPropostaTurma = """
            SELECT CS.ID,
                   CLP.ID AS codafId,
                   CLP.PROPOSTA_ID AS propostaId,
                   CLP.PROPOSTA_TURMA_ID AS propostaTurmaId,
                   CS.DATA_PUBLICACAO AS dataPublicacao,
                   CS.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                   CS.NUMERO_COMUNICADO AS numeroComunicado,
                   CS.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                   CS.CODIGO_CURSO_EOL AS codigoCursoEol,
                   CS.CODIGO_NIVEL AS codigoNivel,
                   CS.OBSERVACAO,
                   CS.STATUS,
                   CS.ALTERADO_EM AS alteradoEm,
                   CS.ALTERADO_POR AS alteradoPor,
                   CS.ALTERADO_LOGIN AS alteradoLogin,
                   CS.CRIADO_EM AS criadoEm,
                   CS.CRIADO_POR AS criadoPor,
                   CS.CRIADO_LOGIN AS criadoLogin,
           
                   P.ID, 
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
           
                   PT.ID, 
                   PT.NOME
            FROM PUBLIC.CODAF_SUPLEMENTAR AS CS
            INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
            INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON CLP.PROPOSTA_TURMA_ID = PT.ID
            INNER JOIN PUBLIC.PROPOSTA AS P ON PT.PROPOSTA_ID = P.ID
            WHERE NOT CS.EXCLUIDO AND NOT PT.EXCLUIDO AND NOT P.EXCLUIDO 
              AND CS.ID = @id;
            """;

        private const string sqlObterRetificacoesPorIdCodaf = """
            SELECT CSR.ID, 
                   CSR.CODAF_SUPLEMENTAR_ID AS CodafSuplementarId,
                   CSR.DATA_RETIFICACAO AS DataRetificacao,
                   CSR.PAGINA_RETIFICACAO_DOM AS PaginaRetificacaoDom,
                   CSR.CRIADO_EM AS CriadoEm,
                   CSR.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_SUPLEMENTAR_RETIFICACAO AS CSR
            WHERE NOT CSR.EXCLUIDO AND CSR.CODAF_SUPLEMENTAR_ID = @id;
            """;

        private const string sqlObterAnexosPorIdCodaf = """
            SELECT CSA.ID, 
                   CSA.CODAF_SUPLEMENTAR_ID AS CodafSuplementarId,
                   CSA.ARQUIVO_CODIGO AS ArquivoCodigo,
                   CSA.NOME_ARQUIVO AS NomeArquivo,
                   CSA.EXTENSAO AS Extensao,
                   CSA.TIPO_ANEXO_ID AS TipoAnexoId,
                   CSA.CRIADO_EM AS CriadoEm,
                   CSA.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_SUPLEMENTAR_ANEXO AS CSA 
            WHERE NOT CSA.EXCLUIDO AND CSA.CODAF_SUPLEMENTAR_ID = @id;
            """;

        private const string sqlCertificadosPorIdCodaf = """
            SELECT CC.ID, 
                   CC.CODIGO_CERTIFICADO AS CodigoCertificado,
                   CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID AS CodafInscricaoListaPresenca,
                   CC.PROPOSTA_REGENTE_TURMA_ID AS PropostaRegenteTurmaId,
                   CC.TIPO_PARTICIPACAO AS TipoParticipacao,
                   CC.STATUS_PROCESSAMENTO AS StatusProcessamento,
                   CC.CRIADO_EM AS CriadoEm,
                   CC.CRIADO_POR AS CriadoPor
            FROM PUBLIC.CODAF_CERTIFICADOS AS CC
            INNER JOIN CODAF_SUPLEMENTAR CS ON CC.CODAF_SUPLEMENTAR_ID = CS.ID
            WHERE NOT CS.EXCLUIDO AND CC.CODAF_SUPLEMENTAR_ID = @id;
            """;

        private const string sqlObterInscricoesDaListaPorIdCodaf = """
            SELECT CSI.ID, 
                   CSI.CODAF_SUPLEMENTAR_ID AS CodafSuplementarId,
                   CSI.INSCRICAO_ID AS InscricaoId,
                   CSI.PERCENTUAL_FREQUENCIA AS PercentualFrequencia,
                   CSI.ATIVIDADE_OBRIGATORIO AS AtividadeObrigatorio,
                   CSI.CONCEITO_FINAL AS ConceitoFinal,
                   CSI.APROVADO AS Aprovado,
                   CSI.CRIADO_EM AS CriadoEm,
                   CSI.CRIADO_POR AS CriadoPor,
                   U.LOGIN,
                   U.CPF,
                   U.NOME
            FROM PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO AS CSI 
                 INNER JOIN PUBLIC.INSCRICAO AS I ON I.ID = CSI.INSCRICAO_ID
                 INNER JOIN PUBLIC.USUARIO AS U  ON U.ID = I.USUARIO_ID 
            WHERE NOT CSI.EXCLUIDO AND CSI.CODAF_SUPLEMENTAR_ID = @id;
            """;

        private const string sqlObterCodafSuplementarPorIdCodafListaPresenca = """
            SELECT CS.ID,
                   CS.CODAF_LISTA_PRESENCA_ID AS CodafId,
                   CS.DATA_PUBLICACAO AS DataPublicacao,
                   CS.DATA_PUBLICACAO_DOM AS DataPublicacaoDom,
                   CS.NUMERO_COMUNICADO AS NumeroComunicado,
                   CS.PAGINA_COMUNICADO_DOM AS PaginaComunicadoDom,
                   CS.CODIGO_CURSO_EOL AS CodigoCursoEol,
                   CS.CODIGO_NIVEL AS CodigoNivel,
                   CS.OBSERVACAO AS Observacao,
                   CS.STATUS AS Status,
                   CS.ALTERADO_EM AS AlteradoEm,
                   CS.ALTERADO_POR AS AlteradoPor,
                   CS.ALTERADO_LOGIN AS AlteradoLogin,
                   CS.CRIADO_EM AS CriadoEm,
                   CS.CRIADO_POR AS CriadoPor,
                   CS.CRIADO_LOGIN AS CriadoLogin,
                   CS.EXCLUIDO AS Excluido
            FROM PUBLIC.CODAF_SUPLEMENTAR AS CS
            WHERE NOT CS.EXCLUIDO 
              AND CS.CODAF_LISTA_PRESENCA_ID = @idCodafListaPresenca;
            """;

        public async Task<DadosPrincipaisRelatorioCodafDto?> ObterDadosRelatorioSuplementarAsync(long codafSuplementarId)
        {
            const string sql = """                
                -- Dados Principais Das Turmas
                SELECT DISTINCT
                        CS.ID AS codafId,
                        PT.ID AS turmaId,
                        PT.NOME AS nomeTurma,
                        p.QUANTIDADE_VAGAS_TURMA AS quantidadeVagasTurma,
                        AP.NOME AS nomeAreaPromotora,
                        P.TIPO_FORMACAO AS tipoFormacao, -- 1-Curso; 2-Evento
                        P.NOME_FORMACAO AS nomeFormacao,
                        P.QUANTIDADE_TURMAS AS quantidadeTurmas,
                        COALESCE(PGP.DATA_INICIO, P.DATA_REALIZACAO_INICIO)
                            AS periodoRealizacaoInicio,
                        COALESCE(PGP.DATA_FIM, P.DATA_REALIZACAO_FIM) 
                            AS periodoRealizacaoFim,
                        P.CURSO_COM_CERTIFICADO AS cursoComCertificado,
                        p.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                        p.CODIGO_EVENTO_SIGPEC AS codigoEventoSigpec,
                        CAST(
                            EXTRACT(HOUR FROM 
                                CASE 
                                    WHEN p.CARGA_HORARIA_TOTAL_OUTRA IS NOT NULL AND p.CARGA_HORARIA_TOTAL_OUTRA <> '' 
                                    THEN p.CARGA_HORARIA_TOTAL_OUTRA::interval
                                    ELSE COALESCE(NULLIF(p.CARGA_HORARIA_PRESENCIAL, ''), '00:00')::interval + 
                                            COALESCE(NULLIF(p.CARGA_HORARIA_DISTANCIA, ''), '00:00')::interval
                                END
                            ) AS INTEGER
                        ) AS cargaHorariaTotal,
                        p.CARGA_HORARIA_DISTANCIA AS cargaHorariaDistancia,
                        p.CARGA_HORARIA_SINCRONA AS cargaHorariaPresencial,
                        p.CARGA_HORARIA_PRESENCIAL AS cargaHorariaSincrona,
                        P.FORMATO AS tipoFormato, -- 1-Presencial;2-A Distância;3-Híbrido
                        CLP.NUMERO_COMUNICADO AS numeroComunicado,
                        CLP.DATA_PUBLICACAO AS dataPublicacao,
                        CLP.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                        clp.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                        CASE
                            WHEN D.DRE_ID IS NULL THEN ''
                            ELSE D.NOME 
                        END AS nomeDre,
                        CLP.Alterado_Em AS DataCodaf,
                        CLP.OBSERVACAO
                FROM   PUBLIC.PROPOSTA AS P
                        INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.PROPOSTA_ID = P.ID
                        INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON AP.ID = P.AREA_PROMOTORA_ID
                        INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
                        INNER JOIN PUBLIC.CODAF_SUPLEMENTAR AS CS ON CS.CODAF_LISTA_PRESENCA_ID = CLP.ID
                        INNER JOIN PUBLIC.PROPOSTA_DRE AS PD ON PD.PROPOSTA_ID = P.ID 
                        INNER JOIN PUBLIC.DRE AS D ON D.ID = PD.DRE_ID 
                        LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO_TURMA PGPT ON PGPT.PROPOSTA_TURMA_ID = PT.ID AND NOT PGPT.EXCLUIDO
                        LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO PGP ON PGP.ID = PGPT.GRUPO_PERIODO_ID AND NOT PGP.EXCLUIDO
                WHERE  CS.ID = @codafSuplementarId;

                -- Data das Aulas
                SELECT PED.DATA_INICIO AS dataInicio,
                        PED.DATA_FIM AS dataFim
                FROM   PUBLIC.CODAF_SUPLEMENTAR AS CS
                        INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.ID = CS.CODAF_LISTA_PRESENCA_ID
                        INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_TURMA AS PET ON PET.TURMA_ID = CLP.PROPOSTA_TURMA_ID
                        INNER JOIN PUBLIC.PROPOSTA_ENCONTRO AS PE ON PE.ID = PET.PROPOSTA_ENCONTRO_ID 
                        INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_DATA AS PED ON PED.PROPOSTA_ENCONTRO_ID = PE.ID 
                WHERE  CS.ID = @codafSuplementarId
                    AND  PE.TIPO IN (@presencial, @sincrono)
                    AND NOT PE.EXCLUIDO 
                    AND NOT PET.EXCLUIDO 
                    AND NOT PED.EXCLUIDO;

                -- Dados dos Regentes
                SELECT coalesce(U.NOME, PR.NOME_REGENTE) AS nome,
                       COALESCE(PR.REGISTRO_FUNCIONAL, PR.CPF) AS registroFuncional,
                       CC.CODIGO_CERTIFICADO AS codigoCertificado
                FROM   PUBLIC.CODAF_SUPLEMENTAR AS CS
                       INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.ID = CS.CODAF_LISTA_PRESENCA_ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = CLP.PROPOSTA_TURMA_ID
                       INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PR.ID = PRT.PROPOSTA_REGENTE_ID 
                       LEFT JOIN PUBLIC.USUARIO AS U ON U.LOGIN = PR.REGISTRO_FUNCIONAL
                       LEFT JOIN PUBLIC.CODAF_CERTIFICADOS AS CC ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                WHERE  CS.ID = @codafSuplementarId;

                -- Dados dos Participantes
                SELECT U.LOGIN AS documento,
                        (U.LOGIN <> U.CPF) AS temRf,
                        U.NOME,
                        CSI.APROVADO,
                        CSI.ATIVIDADE_OBRIGATORIO AS atividadeObrigatoria,
                        CSI.CONCEITO_FINAL AS conceitoFinal,
                        CSI.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
                        CC.CODIGO_CERTIFICADO AS codigoCertificado
                FROM   PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO AS CSI 
                        INNER JOIN PUBLIC.INSCRICAO AS I ON I.ID = CSI.INSCRICAO_ID
                        INNER JOIN PUBLIC.USUARIO AS U ON U.ID = I.USUARIO_ID
                        LEFT JOIN PUBLIC.CODAF_CERTIFICADOS AS CC ON CC.CODAF_SUPLEMENTAR_INSCRICAO_ID = CSI.ID 
                WHERE  NOT CSI.EXCLUIDO 
                    AND  CSI.CODAF_SUPLEMENTAR_ID = @codafSuplementarId
                    AND  NOT U.EXCLUIDO;

                SELECT CRLP.DATA_RETIFICACAO AS DATA, CRLP.PAGINA_RETIFICACAO_DOM AS pagina
                FROM PUBLIC.CODAF_SUPLEMENTAR AS CS
                        INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.ID = CS.CODAF_LISTA_PRESENCA_ID
                        INNER JOIN PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA AS CRLP ON CRLP.CODAF_LISTA_PRESENCA_ID = CLP.ID
                WHERE CS.ID = @codafSuplementarId;                
                """;

            var parametros = new
            {
                codafSuplementarId,
                presencial = (int)TipoEncontro.Presencial,
                sincrono = (int)TipoEncontro.Sincrono
            };

            var conn = conexao.Obter();
            using var multi = await conn.QueryMultipleAsync(sql, parametros);
            var dadosRelatorio = await multi.ReadFirstOrDefaultAsync<DadosPrincipaisRelatorioCodafDto>();

            if (dadosRelatorio == null) return null;

            dadosRelatorio.DataAulas = await multi.ReadAsync<DataAulaTurmaRelatorioCodafDto>();
            dadosRelatorio.RegentesTurma = await multi.ReadAsync<DadosRegenteTurmaRelatorioCodafDto>();
            dadosRelatorio.Participantes = await multi.ReadAsync<DadosParticipanteRelatorioCodafDto>();
            dadosRelatorio.Retificacoes = await multi.ReadAsync<DadosRetificacaoRelatorioCodafDto>();

            return dadosRelatorio;
        }
    }
}