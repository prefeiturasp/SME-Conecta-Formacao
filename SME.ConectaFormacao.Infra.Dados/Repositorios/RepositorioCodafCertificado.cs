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

            using var writer = await ((NpgsqlConnection)conexao.Obter())
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

        public async Task<ResultadoPaginado<MeusCertificadosCodafDto>> ObterMeusCertificadosPorFiltroAsync(FiltroMeusCertificadosCodafDto filtro)
        {
            const string sqlCteBase = CodafCertificadoQueries.ObterMeusCertificadosCteBase;

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

            if (!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
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

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);

            if (totalRegistros == 0)
            {
                return new ResultadoPaginado<MeusCertificadosCodafDto>
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

            var itens = await conn.QueryAsync<MeusCertificadosCodafDto>(sqlConsulta.ToString(), parametros);

            return new ResultadoPaginado<MeusCertificadosCodafDto>
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
                        login = contexto.Permissoes.Any(p => p == Permissao.Codaf_I) ? null : contexto.UsuarioLogado
                    });

        public async Task<ResultadoPaginado<ListagemCertificadosCodafDto>>
            ObterTodosCertificadosAsync(FiltroListagemTodosCertificadosCodafDto filtro)
        {
            const string sqlBaseJoins = CodafCertificadoQueries.ObterTodosCertificadosBaseJoins;
            const string sqlSelect = CodafCertificadoQueries.ObterTodosCertificadosSelect;
            const string sqlOrderBy = "ORDER BY CC.DATA_EMISSAO DESC, CC.CODIGO_CERTIFICADO ASC";

            var condicoesWhere = new StringBuilder("WHERE NOT CC.EXCLUIDO AND  CC.STATUS_PROCESSAMENTO = @processadoComSucesso");
            var parametros = new DynamicParameters();
            parametros.Add("processadoComSucesso", (int)StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso);

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

            if (!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
            {
                condicoesWhere.Append(" AND f_unaccent(P.NOME_FORMACAO) ILIKE f_unaccent(@nomeFormacao) ");
                parametros.Add("nomeFormacao", $"%{filtro.NomeFormacao.Trim()}%");
            }

            if (filtro.PropostaTurmaId.HasValue)
            {
                condicoesWhere.Append(" AND CLP.PROPOSTA_TURMA_ID = @propostaTurmaId ");
                parametros.Add("propostaTurmaId", filtro.PropostaTurmaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoCertificado))
            {
                condicoesWhere.Append(" AND CAST(CC.CODIGO_CERTIFICADO AS TEXT) ILIKE @codigoCertificado ");
                parametros.Add("codigoCertificado", $"{filtro.CodigoCertificado.Trim()}%");
            }

            if (filtro.TipoCertificado == TipoCertificadoCodaf.Cursista)
            {
                condicoesWhere.Append(" AND CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID IS NOT NULL ");
            }
            else if (filtro.TipoCertificado == TipoCertificadoCodaf.Regente)
            {
                condicoesWhere.Append(" AND CC.PROPOSTA_REGENTE_TURMA_ID IS NOT NULL ");
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoCursista))
            {
                condicoesWhere.Append(" AND U_Cursista.LOGIN = @documentoCursista ");
                parametros.Add("documentoCursista", filtro.DocumentoCursista.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoRegente))
            {
                condicoesWhere.Append(" AND PR.REGISTRO_FUNCIONAL = @rfRegente ");
                parametros.Add("rfRegente", filtro.DocumentoRegente.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.NomeCursista))
            {
                condicoesWhere.Append(" AND f_unaccent(U_Cursista.NOME) ILIKE f_unaccent(@nomeCursista) ");
                parametros.Add("nomeCursista", $"%{filtro.NomeCursista.Trim()}%");
            }

            if (filtro.DataEmissao.HasValue)
            {
                // Ajuste para pegar até o final do dia selecionado
                condicoesWhere.Append(" AND CC.DATA_EMISSAO >= @dataEmissaoInicio AND CC.DATA_EMISSAO <= @dataEmissaoFim ");
                parametros.Add("dataEmissaoInicio", filtro.DataEmissao.Value.Date);
                parametros.Add("dataEmissaoFim", filtro.DataEmissao.Value.Date.AddDays(1).AddTicks(-1));
            }

            if (filtro.DreId.HasValue)
            {
                condicoesWhere.Append(" AND PD.DRE_ID = @dreId ");
                parametros.Add("dreId", filtro.DreId.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                SELECT COUNT(1)
                {sqlBaseJoins}
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemCertificadosCodafDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = totalRegistros
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);
            parametros.Add("Cursista", (int)TipoCertificadoCodaf.Cursista);
            parametros.Add("Regente", (int)TipoCertificadoCodaf.Regente);
            parametros.Add("NaoDefinido", (int)TipoCertificadoCodaf.NaoDefinido);

            var sqlConsulta = new StringBuilder($"""
                {sqlSelect}
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens =
                await conn.QueryAsync<ListagemCertificadosCodafDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemCertificadosCodafDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<IList<CodafCertificado>> ObterCertificadosDisponiveisPorListaDeIdAsync(List<long> certificadosId)
        {
            const string sql = """
                SELECT id, 
                       codigo_certificado AS codigoCertificado,
                       codaf_inscricao_lista_presenca_id AS codafInscricaoListaPresencaId,
                       proposta_regente_turma_id AS propostaRegenteTurmaId,
                       tipo_participacao AS tipoParticipacao,
                       data_emissao AS dataEmissao,
                       html_content_snapshot AS htmlContentSnapshot,
                       metadados_json AS metadadosJson,
                       criado_em AS criadoEm,
                       criado_por AS criadoPor,
                       alterado_em AS alteradoEm,
                       alterado_por AS alteradoPor,
                       criado_login AS criadoLogin,
                       alterado_login AS alteradoLogin,
                       excluido AS excluido,
                       status_processamento AS statusProcessamento,
                       chave_objeto_armazenamento AS chaveObjetoArmazenamento,
                       erro_processamento AS erroProcessamento,
                       tentativas_processamento AS tentativasProcessamento,
                       codaf_lista_presenca_id AS codafListaPresencaId
                FROM codaf_certificados
                WHERE id = ANY(@certificadosId) 
                    AND status_processamento = @statusProcessamento 
                    AND NOT excluido
                """;

            var certificados = await conexao.Obter().QueryAsync<CodafCertificado>(sql, new
            {
                certificadosId = certificadosId.ToArray(),
                statusProcessamento = (int)StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso
            });

            return certificados.ToList();
        }

        public async Task AtualizaCodigoCertificado(long codafListaPresencaId)
        {
            await conexao.Obter().ExecuteAsync(
                CodafCertificadoQueries.AtualizarCodigoCertificadoNoHtml,
                new { codafListaPresencaId });
        }
    }
}