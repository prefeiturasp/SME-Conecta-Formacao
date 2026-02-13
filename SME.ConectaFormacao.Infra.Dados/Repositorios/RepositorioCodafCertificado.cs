using Dapper;
using Npgsql;
using NpgsqlTypes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafCertificado(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafCertificado>(contexto, conexao),
        IRepositorioCodafCertificado
    {
        public async Task<IEnumerable<DadosEmissaoCertificadoCodafDto>> 
            ObterDadosParaEmissaoCertificadosCodafAsync(long codafListaPresencaId) => 
            await conexao.Obter()
                .QueryAsync<DadosEmissaoCertificadoCodafDto>(CodafCertificadoQueries.ObterDadosParaEmissao, 
                new { idCodaf = codafListaPresencaId });        

        public async Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados)
        {
            if (certificados is null || !certificados.Any())
                return;

            using var writer = await ((NpgsqlConnection) conexao.Obter())
                .BeginBinaryImportAsync(CodafCertificadoQueries.InserirLoteCopy);
            foreach (var cert in certificados)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(cert.CodafListaPresencaId, NpgsqlDbType.Bigint);

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

        public async Task<IEnumerable<DadosProcessamentoCertificadoCodafDto>> 
            ObterCertificadosParaProcessamentoAsync() => 
            await conexao.Obter().QueryAsync<DadosProcessamentoCertificadoCodafDto>(
                CodafCertificadoQueries.ObterParaProcessamento, new
            {
                statusPendente = (int)StatusProcessamentoCertificadoCodaf.Pendente,
                statusProcessando = (int)StatusProcessamentoCertificadoCodaf.EmProcessamento,
                tamanhoLote = 10
            });

        public async Task AtualizarStatusProcessamentoAsync
            (long id, StatusProcessamentoCertificadoCodaf statusProcessamento, 
            string? chaveObjetoArmazenamento, string? erroProcessamento)
        {
            await conexao.Obter().ExecuteAsync(
                CodafCertificadoQueries.AtualizarStatusProcessamento, 
                new
                {
                    id,
                    statusProcessamento = (int)statusProcessamento,
                    chaveObjetoArmazenamento,
                    erroProcessamento
                });
        }

        public async Task RecuperarCertificadosTravadosAsync()
        {
            await conexao.Obter().ExecuteAsync(CodafCertificadoQueries.RecuperarCertificadosTravados, new
            {
                statusPendente = (int)StatusProcessamentoCertificadoCodaf.Pendente,
                statusProcessando = (int)StatusProcessamentoCertificadoCodaf.EmProcessamento,
                statusErro = (int)StatusProcessamentoCertificadoCodaf.ProcessadoComErro
            });
        }

        public async Task<ResultadoPaginado<ListagemResultadoCertificadoCodafUsuarioDto>> ObterListagemCertificadoDoUsuarioPorFiltroAsync(FiltroListagemResultadoCertificadoCodafUsuarioDto filtro)
        {
            const string sqlCteBase = CodafCertificadoQueries.ObterCertificadosDoUsuarioCteBase;

            var condicoesWhere = new StringBuilder("WHERE LOGIN = @login ");
            var parametros = new DynamicParameters();
            parametros.Add("statusProcessado", (int)StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso);
            parametros.Add("login", contexto.UsuarioLogado);

            if (filtro.CodigoCertificado.HasValue)
            {
                condicoesWhere.Append(" AND codigoCertificado = @codigoCertificado ");
                parametros.Add("codigoCertificado", filtro.CodigoCertificado.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.NumeroHomologacao))
            {
                condicoesWhere.Append(" AND CAST(numeroHomologacao AS TEXT) ILIKE @numeroHomologacao ");
                parametros.Add("numeroHomologacao", $"{filtro.NumeroHomologacao.Trim()}%");
            }

            if(!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
            {
                condicoesWhere.Append(" AND nomeFormacao ILIKE @nomeFormacao ");
                parametros.Add("nomeFormacao", $"%{filtro.NomeFormacao.Trim()}%");
            }

            if (filtro.TipoParticipacao.HasValue)
            {
                condicoesWhere.Append(" AND tipoParticipacao = @tipoParticipacao ");
                parametros.Add("tipoParticipacao", (int)filtro.TipoParticipacao.Value);
            }

            if (filtro.DataEmissaoInicio.HasValue)
            {
                condicoesWhere.Append(" AND dataEmissao >= @dataEmissaoInicio ");
                parametros.Add("dataEmissaoInicio", filtro.DataEmissaoInicio.Value.Date);
            }

            if (filtro.DataEmissaoFim.HasValue)
            {
                // Ajuste para pegar até o final do dia selecionado
                condicoesWhere.Append(" AND dataEmissao <= @dataEmissaoFim ");
                parametros.Add("dataEmissaoFim", filtro.DataEmissaoFim.Value.Date.AddDays(1).AddTicks(-1));
            }

            var conn = conexao.Obter();

            var sqlCount = new StringBuilder($"""
            {sqlCteBase}
            SELECT COUNT(1)
            FROM BaseCertificados
            {condicoesWhere}
            """);

            var qq = sqlCount.ToString();

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);

            if (totalRegistros == 0)
            {
                return new ResultadoPaginado<ListagemResultadoCertificadoCodafUsuarioDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = 0
                };
            }

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);

            const string sqlOrderBy = "ORDER BY dataEmissao DESC, codigoCertificado ASC";

            var sqlConsulta = new StringBuilder($"""
            {sqlCteBase}
            SELECT 
                ID,
                codigoCertificado,
                temRf,
                tipoParticipacao,
                nomeFormacao,
                numeroHomologacao,
                dataEmissao
            FROM BaseCertificados
            {condicoesWhere}
            {sqlOrderBy}
            LIMIT @limite OFFSET @registrosIgnorados
            """);

            var itens = await conn.QueryAsync<ListagemResultadoCertificadoCodafUsuarioDto>(sqlConsulta.ToString(), parametros);

            return new ResultadoPaginado<ListagemResultadoCertificadoCodafUsuarioDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<DadosCertificadoUsuarioParaDownloadDto?> 
            ObterCertificadoDisponivelDoUsuarioAsync(long codafCertificadoId) => 
                await conexao.Obter().QueryFirstOrDefaultAsync<DadosCertificadoUsuarioParaDownloadDto>(
                    CodafCertificadoQueries.ObterCertificadoDisponivelDoUsuario, 
                    new
                    {
                        certificadoId = codafCertificadoId,
                        statusProcessado = (int)StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso,
                        login = contexto.UsuarioLogado
                    });

        public async Task<ResultadoPaginado<ListagemResultadoCertificadoCodafAdminDto>> 
            ObterListagemCertificadoPorFiltroAsync(FiltroListagemResultadoCertificadoCodafAdminDto filtro)
        {
            const string sqlJoinsBase = """
                 FROM   PUBLIC.CODAF_CERTIFICADOS AS CC 
                        INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.id = CC.codaf_lista_presenca_id
                        INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.id = CLP.proposta_turma_id
                        INNER JOIN PUBLIC.PROPOSTA AS P ON P.id = PT.proposta_id
                        INNER JOIN PUBLIC.PROPOSTA_DRE AS PD ON PD.PROPOSTA_ID = P.ID
                        LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP ON CILP.ID = CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID
                        LEFT JOIN PUBLIC.INSCRICAO AS INSCR ON INSCR.ID = CILP.INSCRICAO_ID
                        LEFT JOIN PUBLIC.USUARIO AS U_Cursista  ON U_Cursista.ID = INSCR.USUARIO_ID
                        LEFT JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON CC.PROPOSTA_REGENTE_TURMA_ID = PRT.ID
                        LEFT JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PRT.PROPOSTA_REGENTE_ID = PR.ID
                        LEFT JOIN PUBLIC.USUARIO AS U_Regente ON U_Regente.CPF = PR.REGISTRO_FUNCIONAL OR U_Regente.LOGIN = PR.REGISTRO_FUNCIONAL
                """;
            const string sqlSelect = """
                SELECT CC.ID,
                         CC.CODIGO_CERTIFICADO AS codigoCertificado,
                         coalesce(U_Cursista.NOME, U_Regente.NOME) AS nomeParticipante,
                         CASE
             	            WHEN CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID IS NOT NULL THEN 1 --Cursista
             	            WHEN CC.PROPOSTA_REGENTE_TURMA_ID IS NOT NULL THEN 2 --Regente
             	            ELSE 0 -- Não definido
                         END AS tipoCertificado,
                         coalesce(U_Cursista.LOGIN, U_Regente.LOGIN) AS documento,
                         CC.DATA_EMISSAO AS dataEmissao,
                         P.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                         P.ID AS codigoFormacao,
                         P.NOME_FORMACAO AS nomeFormacao
             """;
            
            const string sqlWhereBase = """
             WHERE  NOT CC.EXCLUIDO 
                AND  CC.STATUS_PROCESSAMENTO = 3 --ProcessadoComSucesso
                AND  P.ID = @codigoFormacao
                AND  P.NUMERO_HOMOLOGACAO = @numeroHomologacao
                AND  P.NOME_FORMACAO ILIKE @nomeFormacao
                AND  CLP.PROPOSTA_TURMA_ID = @propostaTurmaId
                AND  CC.CODIGO_CERTIFICADO = @codigoCertificado
                AND  CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID  IS NOT NULL --Tipo do certificado - Cursista
                AND  CC.PROPOSTA_REGENTE_TURMA_ID IS NOT NULL --Tipo do certificado - Regente
                AND  U_Cursista.LOGIN = @documentoCursista
                AND  U_Regente.LOGIN = @rfRegente
                AND  U_Regente.NOME ILIKE @nomeRegente
                AND  CC.DATA_EMISSAO = @dataEmissao
                AND  PD.DRE_ID = @dreId
            """;
        }
    }
}