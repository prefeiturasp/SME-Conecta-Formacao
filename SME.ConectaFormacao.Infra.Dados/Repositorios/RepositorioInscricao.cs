﻿using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioInscricao : RepositorioBaseAuditavel<Inscricao>, IRepositorioInscricao
    {
        public RepositorioInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> ConfirmarInscricaoVaga(Inscricao inscricao)
        {
            var query = @"update proposta_turma_vaga set inscricao_id = @Id
                          where id = (
                                select id 
                                from proposta_turma_vaga 
                                where proposta_turma_id = @PropostaTurmaId 
                                  and inscricao_id is null limit 1)";

            return await conexao.Obter().ExecuteAsync(query, inscricao) > 0;
        }

        public Task<bool> ExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var situacaoCancelada = (int)SituacaoInscricao.Cancelada;

            var query = @"select 1 
                          from inscricao i 
                          inner join proposta_turma pt on pt.id = i.proposta_turma_id and not pt.excluido
                          where i.usuario_id = @usuarioId 
                            and pt.proposta_id = @propostaId
                            and i.situacao <> @situacaoCancelada
                            and not i.excluido
                          limit 1";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { propostaId, usuarioId, situacaoCancelada });
        }

        public Task<int> LiberarInscricaoVaga(Inscricao inscricao)
        {
            var query = @"update proposta_turma_vaga set inscricao_id = null
                          where id = @id";

            return conexao.Obter().ExecuteAsync(query, inscricao);
        }
    }
}
