using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class ValidadorCodafListaPresencaService(
        IRepositorioProposta repositorioProposta,
        IRepositorioCodafListaPresenca repositorioLista) : IValidadorCodafListaPresencaService
    {
        public async Task<Erro?> ValidarUnicidadeTurmaListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0)
        {
            var jaPossuiLista = await repositorioLista.TurmaJaTemListaDePresencaAsync(propostaTurmaId, listaPresencaId);

            if (jaPossuiLista)
                return Erro.Negocio($"Já existe uma lista de presença cadastrada para esta turma.");

            return null;
        }

        public async Task<Erro?> ValidarVinculoPropostaTurmaAsync(long propostaId, long propostaTurmaId)
        {
            var proposta = await repositorioProposta.ObterPorId(propostaId);
            if (proposta is null)
                return Erro.Validacao("Proposta não encontrada.");

            var turma = await repositorioProposta.ObterTurmaPorId(propostaTurmaId);
            if (turma is null)
                return Erro.Validacao("Turma não encontrada.");

            if (turma.PropostaId != proposta.Id)
                return Erro.Validacao("A turma informada não pertence à formação selecionada.");

            return null;
        }
    }
}
