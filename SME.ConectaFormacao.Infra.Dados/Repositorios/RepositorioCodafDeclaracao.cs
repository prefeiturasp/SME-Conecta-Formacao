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
                new { codafNaoHomologadoId = codafNaoHomologadoId });

        public async Task InserirLoteAsync(IEnumerable<CodafDeclaracao> declaracoes)
        {
            if (declaracoes is null || !declaracoes.Any())
                return;

            // Obter próximos valores das sequências
            var proximoId = await conexao.Obter()
                .QuerySingleAsync<long>("SELECT COALESCE(MAX(id), 0) + 1 FROM public.codaf_declaracoes");

            var proximoCodigoDeclaracao = await conexao.Obter()
                .QuerySingleAsync<long>("SELECT COALESCE(MAX(codigo_declaracao), 0) + 1 FROM public.codaf_declaracoes");

            using var writer = await ((NpgsqlConnection)conexao.Obter())
                .BeginBinaryImportAsync(CodafDeclaracaoQueries.InserirLoteCopy);

            var criadoEm = DateTimeExtension.HorarioBrasilia();
            var nomeUsuario = contexto.NomeUsuario;
            var usuarioLogado = contexto.UsuarioLogado;
            var id = proximoId;
            var codigoDeclaracao = proximoCodigoDeclaracao;

            foreach (var declaracao in declaracoes)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(id, NpgsqlDbType.Bigint);
                await writer.WriteAsync(codigoDeclaracao, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(declaracao.CodafCursoNaoHomologadoInscricaoId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(declaracao.CodafCursoNaoHomologadoId, NpgsqlDbType.Bigint);
                await writer.EscreverNuloOuValorAsync(declaracao.PropostaRegenteTurmaId, NpgsqlDbType.Bigint);

                await writer.WriteAsync((int)declaracao.TipoParticipacao, NpgsqlDbType.Integer);
                await writer.WriteAsync(declaracao.DataEmissao, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(declaracao.HtmlContentSnapshot, NpgsqlDbType.Text);
                await writer.EscreverNuloOuStringAsync(declaracao.MetadadosJson, NpgsqlDbType.Jsonb);

                await writer.WriteAsync((int)declaracao.StatusProcessamento, NpgsqlDbType.Integer);
                await writer.WriteAsync(0, NpgsqlDbType.Integer);
                await writer.WriteAsync(criadoEm, NpgsqlDbType.Timestamp);
                await writer.WriteAsync(nomeUsuario, NpgsqlDbType.Varchar);
                await writer.WriteAsync(usuarioLogado, NpgsqlDbType.Varchar);
                await writer.WriteAsync(false, NpgsqlDbType.Boolean);

                id++;
                codigoDeclaracao++;
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

        public async Task<IList<CodafDeclaracao>> ObterDeclaracoesDisponiveisPorListaDeIdAsync(List<long> declaracoesId)
        {
            const string sql = """
                SELECT id, 
                       codigo_declaracao AS codigoDeclaracao,
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
                FROM codaf_declaracaos
                WHERE id = ANY(@declaracaosId) 
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
    }
}