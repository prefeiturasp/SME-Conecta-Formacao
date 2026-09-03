using Dapper;
using Npgsql;
using NpgsqlTypes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafDeclaracao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafDeclaracao>(contexto, conexao),
        IRepositorioCodafDeclaracao
    {
        public async Task<IEnumerable<DadosEmissaoDeclaracaoCodafDto>>
            ObterDadosParaEmissaoDeclaracoesCodafAsync(long codafNaoHomologadoId) =>
            await conexao.Obter()
                .QueryAsync<DadosEmissaoDeclaracaoCodafDto>(CodafDeclaracaoQueries.ObterDadosParaEmissao,
                new { codafNaoHomologadoId });

        public async Task InserirLoteAsync(IEnumerable<CodafDeclaracao> declaracoes)
        {
            if (declaracoes is null || !declaracoes.Any())
                return;

            using var writer = await ((NpgsqlConnection)conexao.Obter())
                .BeginBinaryImportAsync(CodafDeclaracaoQueries.InserirLoteCopy);

            var criadoEm = DateTimeExtension.HorarioBrasilia();
            var nomeUsuario = contexto.NomeUsuario;
            var usuarioLogado = contexto.UsuarioLogado;

            foreach (var declaracao in declaracoes)
            {
                await writer.StartRowAsync();

                await writer.EscreverNuloOuValorAsync(declaracao.CodafCursoNaoHomologadoInscricaoId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(declaracao.CodafCursoNaoHomologadoId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(declaracao.PropostaRegenteTurmaId, NpgsqlDbType.Bigint);

                await writer.WriteAsync((int)declaracao.TipoParticipacao, NpgsqlDbType.Integer);
                await writer.WriteAsync(declaracao.DataEmissao, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(declaracao.HtmlContentSnapshot, NpgsqlDbType.Text);
                await writer.EscreverNuloOuStringAsync(declaracao.MetadadosJson, NpgsqlDbType.Jsonb);

                await writer.WriteAsync((int)declaracao.StatusProcessamento, NpgsqlDbType.Integer);
                await writer.WriteAsync(0, NpgsqlDbType.Integer); // tentativasProcessamento
                await writer.WriteAsync(criadoEm, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(nomeUsuario, NpgsqlDbType.Varchar);
                await writer.WriteAsync(usuarioLogado, NpgsqlDbType.Varchar);
                await writer.WriteAsync(false, NpgsqlDbType.Boolean); // excluido
            }
            await writer.CompleteAsync();
        }

        public async Task AtualizarStatusProcessamentoAsync
            (long id, StatusProcessamentoDeclaracaoCodaf statusProcessamento,
            string? chaveObjetoArmazenamento, string? erroProcessamento)
        {
            await conexao.Obter().ExecuteAsync(
                CodafDeclaracaoQueries.AtualizarStatusProcessamento,
                new
                {
                    id,
                    statusProcessamento = (int)statusProcessamento,
                    chaveObjetoArmazenamento,
                    erroProcessamento
                });
        }

        public async Task<ResultadoPaginado<ListagemDeclaracoesCodafDto>>
           ObterTodasDeclaracoesAsync(FiltroListagemTodasDeclaracoesCodafDto filtro)
        {
            var condicoesWhere = new StringBuilder("WHERE 1=1 ");
            var parametros = new DynamicParameters();
            parametros.Add("processadoComSucesso", (int)StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso);
            parametros.Add("Cursista", (int)TipoDeclaracaoCodaf.Cursista);
            parametros.Add("Regente", (int)TipoDeclaracaoCodaf.Regente);

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

            if (!string.IsNullOrWhiteSpace(filtro.CodigoDeclaracao))
            {
                condicoesWhere.Append(" AND CAST(codigoDeclaracao AS TEXT) ILIKE @codigoDeclaracao ");
                parametros.Add("codigoDeclaracao", $"{filtro.CodigoDeclaracao.Trim()}%");
            }

            if (filtro.TipoDeclaracao == TipoDeclaracaoCodaf.Cursista || filtro.TipoDeclaracao == TipoDeclaracaoCodaf.Regente)
            {
                condicoesWhere.Append(" AND tipoDeclaracao = @tipoDeclaracaoFiltro ");
                parametros.Add("tipoDeclaracaoFiltro", (int)filtro.TipoDeclaracao);
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoCursista))
            {
                condicoesWhere.Append(" AND documento = @documentoCursista AND tipoDeclaracao = @Cursista ");
                parametros.Add("documentoCursista", filtro.DocumentoCursista.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.DocumentoRegente))
            {
                condicoesWhere.Append(" AND documento = @documentoRegente AND tipoDeclaracao = @Regente ");
                parametros.Add("documentoRegente", filtro.DocumentoRegente.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtro.NomeCursista))
            {
                condicoesWhere.Append(" AND f_unaccent(nomeCursista) ILIKE f_unaccent(@nomeCursista) AND tipoDeclaracao = @Cursista ");
                parametros.Add("nomeCursista", $"%{filtro.NomeCursista.Trim()}%");
            }

            if (filtro.DataEmissao.HasValue)
            {
                condicoesWhere.Append(" AND CAST(dataEmissao AS DATE) = CAST(@dataEmissao AS DATE) ");
                parametros.Add("dataEmissao", filtro.DataEmissao.Value);
            }

            if (filtro.EmissorId.HasValue)
            {
                condicoesWhere.Append(" AND emissorId = @emissorId");
                parametros.Add("emissorId", filtro.EmissorId.Value);               
            }

            if (filtro.TurmaId.HasValue)
            {
                condicoesWhere.Append(" AND turmaId = @turmaId ");
                parametros.Add("turmaId", filtro.TurmaId.Value);
            }

            var conn = conexao.Obter();
            var sqlCount = new StringBuilder($"""
                {CodafDeclaracaoQueries.ObterTodasDeclaracoesCteBase}
                SELECT COUNT(1)
                FROM BaseDeclaracoes
                {condicoesWhere}
                """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<ListagemDeclaracoesCodafDto>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = totalRegistros
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);

            var sqlConsulta = new StringBuilder($"""
                {CodafDeclaracaoQueries.ObterTodasDeclaracoesCteBase}
                SELECT 
                    id, 
                    codigoDeclaracao, 
                    nomeCursista, 
                    nomeRegente,
                    tipoDeclaracao, 
                    CASE WHEN tipoDeclaracao = 1 THEN documento ELSE NULL END AS documentoCursista,
                    CASE WHEN tipoDeclaracao = 2 THEN documento ELSE NULL END AS documentoRegente,
                    dataEmissao, 
                    numeroHomologacao, 
                    codigoFormacao, 
                    nomeFormacao, 
                    tipoEmissor, 
                    emissorId, 
                    nomeEmissor, 
                    turmaId
                FROM BaseDeclaracoes
                {condicoesWhere}
                ORDER BY dataEmissao DESC, codigoDeclaracao ASC
                LIMIT @limite OFFSET @registrosIgnorados
                """);

            var itens =
                await conn.QueryAsync<ListagemDeclaracoesCodafDto>(sqlConsulta.ToString(), parametros);
            return new ResultadoPaginado<ListagemDeclaracoesCodafDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<IList<CodafDeclaracao>> ObterDeclaracoesDisponiveisPorListaDeIdAsync(List<long> declaracoesId)
        {
            const string sql = """
                SELECT id, 
                       codigo_declaracao AS codigoDeclaracao,
                       codaf_curso_nao_homologado_inscricao_id AS CodafCursoNaoHomologadoInscricaoId,
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
                       codaf_curso_nao_homologado_id AS CodafCursoNaoHomologadoId
                FROM codaf_declaracoes
                WHERE id = ANY(@declaracoesId) 
                    AND status_processamento = @statusProcessamento 
                    AND NOT excluido
                """;

            var declaracoes = await conexao.Obter().QueryAsync<CodafDeclaracao>(sql, new
            {
                declaracoesId = declaracoesId.ToArray(),
                statusProcessamento = (int)StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso
            });

            return declaracoes.ToList();
        }

        public async Task AtualizaCodigoDeclaracao(long codafNaoHomologadoId)
        {
            await conexao.Obter().ExecuteAsync(
                CodafDeclaracaoQueries.AtualizarCodigoDeclaracaoNoHtml,
                new { codafNaoHomologadoId });
        }

        public async Task<IEnumerable<DadosProcessamentoCodafDto>>
          ObterDeclaracoesParaProcessamentoAsync() =>
          await conexao.Obter().QueryAsync<DadosProcessamentoCodafDto>(
              CodafDeclaracaoQueries.ObterParaProcessamento, new
              {
                  statusPendente = (int)StatusProcessamentoDeclaracaoCodaf.Pendente,
                  statusProcessando = (int)StatusProcessamentoDeclaracaoCodaf.EmProcessamento,
                  tamanhoLote = 10
              });

        public async Task InativarDeclaracoesAnterioresCursistaAsync(IEnumerable<long> idInscritos)
        {
            await conexao.Obter().ExecuteAsync(
                CodafDeclaracaoQueries.InativarDeclaracoesAnterioresDeCursistas,
                new { inscricaoId = idInscritos.ToArray(), usuarioNome = contexto.NomeUsuario, usuarioLogin = contexto.UsuarioLogado });
        }

        public async Task<ResultadoPaginado<MinhasDeclaracoesCodafDto>> ObterMinhasDeclaracoesPorFiltroAsync(FiltroMinhasDeclaracoesCodafDto filtro)
        {
            const string sqlCteBase = CodafDeclaracaoQueries.ObterMinhasDeclaracoesCteBase;

            var condicoesWhere = new StringBuilder("WHERE LOGIN = @login ");
            var parametros = new DynamicParameters();
            parametros.Add("statusProcessado", (int)StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso);
            parametros.Add("login", contexto.UsuarioLogado);

            if (filtro.CodigoDeclaracao.HasValue)
            {
                condicoesWhere.Append(" AND codigoDeclaracao = @codigoDeclaracao ");
                parametros.Add("codigoDeclaracao", filtro.CodigoDeclaracao.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.CodigoFormacao))
            {
                condicoesWhere.Append(" AND CAST(codigoFormacao AS TEXT) ILIKE @codigoFormacao ");
                parametros.Add("codigoFormacao", $"{filtro.CodigoFormacao.Trim()}%");
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
            FROM BaseDeclaracoes
            {condicoesWhere}
            """);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);

            if (totalRegistros == 0)
            {
                return new ResultadoPaginado<MinhasDeclaracoesCodafDto>
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

            const string sqlOrderBy = "ORDER BY dataEmissao DESC, codigoDeclaracao ASC";

            var sqlConsulta = new StringBuilder($"""
            {sqlCteBase}
            SELECT 
                ID,
                codigoDeclaracao,
                temRf,
                tipoParticipacao,
                nomeFormacao,
                codigoFormacao,
                dataEmissao
            FROM BaseDeclaracoes
            {condicoesWhere}
            {sqlOrderBy}
            LIMIT @limite OFFSET @registrosIgnorados
            """);

            var itens = await conn.QueryAsync<MinhasDeclaracoesCodafDto>(sqlConsulta.ToString(), parametros);

            return new ResultadoPaginado<MinhasDeclaracoesCodafDto>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<DadosDeclaracaoUsuarioParaDownloadDto?> 
            ObterDeclaracaoDisponivelDoUsuarioAsync(long codafDeclaracaoId) =>
                await conexao.Obter().QueryFirstOrDefaultAsync<DadosDeclaracaoUsuarioParaDownloadDto>(
                    CodafDeclaracaoQueries.ObterDeclaracaoDisponivelDoUsuario,
                    new
                    {
                        declaracaoId = codafDeclaracaoId,
                        statusProcessado = (int)StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso,
                        login = contexto.Permissoes.Any(p => p == Permissao.Codaf_I) ? null : contexto.UsuarioLogado
                    });
    }
}