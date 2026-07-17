using Dapper;
using Npgsql;
using NpgsqlTypes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Extensoes;
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


        public async Task<IEnumerable<DadosEmissaoCertificadoCodafDto>>
            ObterDadosParaEmissaoCertificadosCodafSuplementarAsync(long codafSuplementarId) =>
            await conexao.Obter()
                .QueryAsync<DadosEmissaoCertificadoCodafDto>(CodafCertificadoQueries.ObterDadosParaEmissaoSuplementar,
                new { codafSuplementarId });

        public async Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados)
        {
            if (certificados is null || !certificados.Any())
                return;

            using var writer = await ((NpgsqlConnection)conexao.Obter())
                .BeginBinaryImportAsync(CodafCertificadoQueries.InserirLoteCopy);

            var criadoEm = DateTimeExtension.HorarioBrasilia();
            var nomeUsuario = contexto.NomeUsuario;
            var usuarioLogado = contexto.UsuarioLogado;

            foreach (var cert in certificados)
            {
                await writer.StartRowAsync();

                await writer.EscreverNuloOuValorAsync(cert.CodafListaPresencaId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(cert.CodafSuplementarId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(cert.CodafInscricaoListaPresencaId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(cert.CodafSuplementarInscricaoId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(cert.PropostaRegenteTurmaId, NpgsqlDbType.Bigint);

                await writer.WriteAsync((int)cert.TipoParticipacao, NpgsqlDbType.Integer);
                await writer.WriteAsync(cert.DataEmissao, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(cert.HtmlContentSnapshot, NpgsqlDbType.Text);

                await writer.EscreverNuloOuStringAsync(cert.MetadadosJson, NpgsqlDbType.Jsonb);

                await writer.WriteAsync(criadoEm, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(nomeUsuario, NpgsqlDbType.Varchar);
                await writer.WriteAsync(usuarioLogado, NpgsqlDbType.Varchar);
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
            var condicoesWhere = new StringBuilder("WHERE 1=1 ");
            var parametros = new DynamicParameters();
            parametros.Add("processadoComSucesso", (int)StatusProcessamentoCertificadoCodaf.ProcessadoComSucesso);
            parametros.Add("Cursista", (int)TipoCertificadoCodaf.Cursista);
            parametros.Add("Regente", (int)TipoCertificadoCodaf.Regente);

            if (!string.IsNullOrWhiteSpace(filtro.CodigoFormacao))
            {
                condicoesWhere.Append(" AND CAST(codigoFormacao AS TEXT) ILIKE @codigoFormacao ");
                parametros.Add("codigoFormacao", $"{filtro.CodigoFormacao.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.NumeroHomologacao))
            {
                condicoesWhere.Append(" AND CAST(numeroHomologacao AS TEXT) ILIKE @numeroHomologacao ");
                parametros.Add("numeroHomologacao", $"{filtro.NumeroHomologacao.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.NomeFormacao))
            {
                condicoesWhere.Append(" AND f_unaccent(nomeFormacao) ILIKE f_unaccent(@nomeFormacao) ");
                parametros.Add("nomeFormacao", $"%{filtro.NomeFormacao.Trim()}%");
            }

            if (filtro.PropostaTurmaId.HasValue)
            {
                condicoesWhere.Append(" AND propostaTurmaId = @propostaTurmaId ");
                parametros.Add("propostaTurmaId", filtro.PropostaTurmaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoCertificado))
            {
                condicoesWhere.Append(" AND CAST(codigoCertificado AS TEXT) ILIKE @codigoCertificado ");
                parametros.Add("codigoCertificado", $"{filtro.CodigoCertificado.Trim()}%");
            }

            if (filtro.TipoCertificado == TipoCertificadoCodaf.Cursista || filtro.TipoCertificado == TipoCertificadoCodaf.Regente)
            {
                condicoesWhere.Append(" AND tipoCertificado = @tipoCertificadoFiltro ");
                parametros.Add("tipoCertificadoFiltro", (int)filtro.TipoCertificado);
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoCursista))
            {
                condicoesWhere.Append(" AND documento = @documentoCursista AND tipoCertificado = @Cursista ");
                parametros.Add("documentoCursista", filtro.DocumentoCursista.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoRegente))
            {
                condicoesWhere.Append(" AND documento = @documentoRegente AND tipoCertificado = @Regente ");
                parametros.Add("documentoRegente", filtro.DocumentoRegente.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.NomeCursista))
            {
                condicoesWhere.Append(" AND f_unaccent(nomeParticipante) ILIKE f_unaccent(@nomeCursista) AND tipoCertificado = @Cursista ");
                parametros.Add("nomeCursista", $"%{filtro.NomeCursista.Trim()}%");
            }

            if (filtro.DataEmissao.HasValue)
            {
                condicoesWhere.Append(" AND dataEmissao >= @dataEmissaoInicio AND dataEmissao <= @dataEmissaoFim ");
                parametros.Add("dataEmissaoInicio", filtro.DataEmissao.Value.Date);
                parametros.Add("dataEmissaoFim", filtro.DataEmissao.Value.Date.AddDays(1).AddTicks(-1));
            }

            if (filtro.DreId.HasValue)
            {
                condicoesWhere.Append(" AND dreId = @dreId ");
                parametros.Add("dreId", filtro.DreId.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                {CodafCertificadoQueries.ObterTodosCertificadosCteBase}
                SELECT COUNT(1)
                FROM BaseCertificados
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
                {CodafCertificadoQueries.ObterTodosCertificadosCteBase}
                SELECT 
                    id, codigoCertificado, nomeParticipante, tipoCertificado,
                    documento, dataEmissao, numeroHomologacao, codigoFormacao, nomeFormacao
                FROM BaseCertificados
                {condicoesWhere}
                ORDER BY dataEmissao DESC, codigoCertificado ASC
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

        public async Task AtualizaCodigoCertificado(long codafId, TipoCodaf tipoCodaf)
        {
            await conexao.Obter().ExecuteAsync(
                CodafCertificadoQueries.AtualizarCodigoCertificadoNoHtml,
                new { codafId, tipoCodaf });
        }

        public async Task InativarCertificadosAnterioresCursistaAsync(IEnumerable<long> idInscritos)
        {
            await conexao.Obter().ExecuteAsync(
                CodafCertificadoQueries.InativarCertificadosAnterioresDeCursistas,
                new { inscricaoId = idInscritos.ToArray(), usuarioNome = contexto.NomeUsuario, usuarioLogin = contexto.UsuarioLogado });
        }
    }
}