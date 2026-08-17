using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Globalization;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoObterDetalhesPropostaComTurmasPorId(IRepositorioProposta repositorioProposta) : ICasoDeUsoObterDetalhesPropostaComTurmasPorId
    {
        public async Task<Resultado<PropostaComTurmasDto?>> ExecutarAsync(long propostaId, bool formacoesHomologadas)
        {
            var proposta = await repositorioProposta.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas);

            if (proposta is null)
                return Resultado<PropostaComTurmasDto?>.DeSucesso(null);

            StringComparer numComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering);
                proposta.Turmas = [.. proposta.Turmas.OrderBy(x => x.Nome, numComparer)];

            return proposta;
        }
    }
}
