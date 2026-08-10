using Dapper;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Queries;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioRelatorios(IConectaFormacaoConexao conexao) : IRepositorioRelatorios
    {
        private static readonly TimeZoneInfo TimezoneBrasilia =
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public async Task<IEnumerable<InscritoFormacaoQueryModel>> ObterDadosRelatorioInscritosPorFormacaoAsync(
            FiltroRelatorioInscritosPorFormacaoDto filtro)
        {
            var parametros = new DynamicParameters();
            var condicoesWhere = ConstruirCondicoesFiltro(filtro, parametros);

            var sqlConsulta = new StringBuilder(
            $"""
            {RelatoriosInscritosQueries.ObterInscritosPorFormacao}
            {condicoesWhere}
            {RelatoriosInscritosQueries.QueryOrderbyInscritosPorFormacao}
            """);

            var conn = conexao.Obter();
            return await conn.QueryAsync<InscritoFormacaoQueryModel>(sqlConsulta.ToString(), parametros);
        }

        private static string ConstruirCondicoesFiltro(FiltroRelatorioInscritosPorFormacaoDto filtro, DynamicParameters parametros)
        {
            var condicoes = new StringBuilder(" WHERE rn = 1 ");

            if (filtro.PeriodoDeRealizacaoInicial.Year >= 2000 && filtro.PeriodoDeRealizacaoFinal.Year >= 2000)
            {
                AdicionarFiltroData(
                    condicoes,
                    parametros,
                    filtro.PeriodoDeRealizacaoInicial,
                    " AND dataRealizacaoInicio::date >= @periodoDeRealizacaoInicial ",
                    "periodoDeRealizacaoInicial"
                );

                AdicionarFiltroData(
                    condicoes,
                    parametros,
                    filtro.PeriodoDeRealizacaoFinal,
                    " AND dataRealizacaoFim::date <= @periodoDeRealizacaoFinal ",
                    "periodoDeRealizacaoFinal"
                );
            }

            AdicionarFiltroOpcional(condicoes, parametros, filtro.PropostaId, " AND codigoFormacao = @propostaId ", "propostaId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.NumeroHomologacao, " AND codigoHomologacao = @numeroHomologacao ", "numeroHomologacao");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.PropostaTurmaId, " AND turma = @propostaTurmaId ", "propostaTurmaId");

            AdicionarFiltroOpcional(condicoes, parametros, filtro.AreaPromotoraId, " AND areaPromotoraId = @areaPromotoraId ", "areaPromotoraId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.DreId, " AND dreId = @dreId ", "dreId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.UeId, " AND ueId = @ueId ", "ueId");

            AdicionarFiltroOpcional(condicoes, parametros, (int?)filtro.SituacaoProposta, " AND situacaoFormacao = @situacaoFormacao ", "situacaoFormacao");
            AdicionarFiltroOpcional(condicoes, parametros, (int?)filtro.SituacaoInscricao, " AND situacaoInscricao = @situacaoInscricao ", "situacaoInscricao");
            AdicionarFiltroOpcional(condicoes, parametros, (int?)filtro.Formato, " AND modalidadeFormativa = @formato ", "formato");
            AdicionarFiltroOpcional(condicoes, parametros, (int?)filtro.Modalidade, " AND etapaModalidade = @modalidade ", "modalidade");

            AdicionarFiltroOpcional(condicoes, parametros, filtro.CargoPublicoAlvoId, " AND publicoAlvo = @cargoPublicoAlvoId ", "cargoPublicoAlvoId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.FuncaoId, " AND funcaoEspecifica = @funcaoId ", "funcaoId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.AnoTurmaId, " AND anoEtapa = @anoTurmaId ", "anoTurmaId");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.ComponenteCurricularId, " AND componenteCurricular = @componenteCurricularId ", "componenteCurricularId");

            AdicionarFiltroOpcional(condicoes, parametros, filtro.Pcd, " AND pcd = @pcd ", "pcd");
            AdicionarFiltroOpcional(condicoes, parametros, filtro.NecessitaAdaptacao, " AND necessitaAdaptacao = @necessitaAdaptacao ", "necessitaAdaptacao");

            AdicionarFiltroTexto(condicoes, parametros, filtro.NomeFormacao, " AND f_unaccent(nomeFormacao) ILIKE f_unaccent(@nomeFormacao) ", "nomeFormacao");
            AdicionarFiltroTexto(condicoes, parametros, filtro.Email, " AND f_unaccent(email) ILIKE f_unaccent(@email) ", "email");
            AdicionarFiltroTexto(condicoes, parametros, filtro.DocumentoCursista, " AND rfCpf = @documentoCursista ", "documentoCursista", buscaParcial: false);

            return condicoes.ToString();
        }

        private static void AdicionarFiltroOpcional<T>(StringBuilder query, DynamicParameters parametros, T? valor, string sql, string nomeParametro) where T : struct
        {
            if (valor.HasValue)
            {
                query.Append(sql);
                parametros.Add(nomeParametro, valor.Value);
            }
        }

        private static void AdicionarFiltroData(StringBuilder query, DynamicParameters parametros, DateTime valor, string sql, string nomeParametro)
        {
            query.Append(sql);

            var dataLocal = valor.Kind == DateTimeKind.Utc
                ? TimeZoneInfo.ConvertTimeFromUtc(valor, TimezoneBrasilia).Date
                : valor.Date;

            parametros.Add(nomeParametro, DateOnly.FromDateTime(dataLocal), DbType.Date);
        }

        private static void AdicionarFiltroTexto(StringBuilder query, DynamicParameters parametros, string? valor, string sql, string nomeParametro, bool buscaParcial = true)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                query.Append(sql);
                var valorTratado = buscaParcial ? $"%{valor.Trim()}%" : valor.Trim();
                parametros.Add(nomeParametro, valorTratado);
            }
        }
    }
}
