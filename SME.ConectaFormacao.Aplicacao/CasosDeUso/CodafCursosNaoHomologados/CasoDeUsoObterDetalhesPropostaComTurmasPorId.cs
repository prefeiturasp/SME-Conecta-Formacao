using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Globalization;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoObterDetalhesPropostaComTurmasPorId(
        IRepositorioProposta repositorioProposta,
        IServicoPeriodoEncontroProposta servicoPeriodoEncontroProposta) : ICasoDeUsoObterDetalhesPropostaComTurmasPorId
    {
        public async Task<Resultado<PropostaComTurmasDto?>> ExecutarAsync(long propostaId, bool formacoesHomologadas)
        {
            var proposta = await repositorioProposta.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas);

            if (proposta is null)
                return Resultado<PropostaComTurmasDto?>.DeSucesso(null);

            if (proposta.Turmas != null)
            {
                proposta.Turmas = proposta.Turmas.DistinctBy(t => t.Id).ToList();

                foreach (var turma in proposta.Turmas)
                {
                    var periodo = await servicoPeriodoEncontroProposta.ObterPeriodoEncontrosTurmaAsync(turma.Id);
                    turma.Descricao = $"{turma.Nome}{periodo}".TrimEnd();
                }

                StringComparer numComparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering);
                proposta.Turmas = [.. proposta.Turmas.OrderBy(x => x.Nome, numComparer)];
            }

            return proposta;
        }
    }
}
