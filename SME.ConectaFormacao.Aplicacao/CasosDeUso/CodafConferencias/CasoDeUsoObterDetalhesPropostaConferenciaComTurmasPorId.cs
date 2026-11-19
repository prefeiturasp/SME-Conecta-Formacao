using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Globalization;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId(
        IRepositorioCodafConferencia repositorioCodafConferencia) : ICasoDeUsoObterDetalhesPropostaConferenciaComTurmasPorId
    {
        public async Task<Resultado<PropostaComTurmasDto?>> ExecutarAsync(long propostaId)
        {
            var proposta = await repositorioCodafConferencia.ObterDetalhesPropostaConferenciaComTurmasPorIdAsync(propostaId);

            if (proposta is null)
                return Resultado<PropostaComTurmasDto?>.DeSucesso(null);

            var numComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering);
            proposta.Turmas = [.. proposta.Turmas.OrderBy(x => x.Nome, numComparer)];

            return proposta;
        }
    }
}
