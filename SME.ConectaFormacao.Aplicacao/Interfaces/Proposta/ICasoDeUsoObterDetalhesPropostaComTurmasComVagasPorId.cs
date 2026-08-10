using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterDetalhesPropostaComTurmasPorId
    {
        Task<Resultado<PropostaComTurmasDto?>> ExecutarAsync(long propostaId, bool formacoesHomologadas);
    }
}