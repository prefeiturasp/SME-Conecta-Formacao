using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPeriodoRealizacaoConsulta(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : RepositorioBaseAuditavel<Proposta>(contexto, conexao), IRepositorioPeriodoRealizacaoConsulta
    {
        public async Task<PeriodoRealizacao?> ObterPeriodoRealizacaoAsync(long propostaTurmaId)
        {
            var query = @"
            SELECT
                COALESCE(pgp.data_inicio, p.data_realizacao_inicio) AS DataInicio,
                COALESCE(pgp.data_fim, p.data_realizacao_fim) AS DataFim
            FROM proposta_turma pt
            INNER JOIN proposta p 
                ON p.id = pt.proposta_id
            LEFT JOIN proposta_grupo_periodo_turma pgpt 
                ON pgpt.proposta_turma_id = pt.id 
                AND NOT pgpt.excluido
            LEFT JOIN proposta_grupo_periodo pgp 
                ON pgp.id = pgpt.grupo_periodo_id 
                AND NOT pgp.excluido
            WHERE pt.id = @propostaTurmaId
              AND NOT pt.excluido
            LIMIT 1;";

            return await conexao.Obter()
                .QueryFirstOrDefaultAsync<PeriodoRealizacao>(
                    query,
                    new { propostaTurmaId });
        }
    }
}