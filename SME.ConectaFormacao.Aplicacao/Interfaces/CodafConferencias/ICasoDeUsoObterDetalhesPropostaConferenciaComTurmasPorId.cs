using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId
    {
        Task<Resultado<PropostaComTurmasDto?>> ExecutarAsync(long propostaId);
    }
}
