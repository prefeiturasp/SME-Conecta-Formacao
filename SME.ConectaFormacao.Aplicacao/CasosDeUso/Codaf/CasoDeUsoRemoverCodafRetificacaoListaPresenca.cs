using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoRemoverCodafRetificacaoListaPresenca(IRepositorioCodafRetificacaoListaPresenca repositorioCodafRetificacaoListaPresenca, IRepositorioCodafListaPresenca repositorioCodafListaPresenca) :
        ICasoDeUsoRemoverCodafRetificacaoListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafRetificacaoListaPresencaId)
        {
            var retificacao = await repositorioCodafRetificacaoListaPresenca.ObterNaoExcluidosPorIdAsync(codafRetificacaoListaPresencaId);
            if (retificacao is null)
                return Erro.NaoEncontrado("Retificação não encontrada.");

            var codaf = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId);
            if (codaf is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (codaf.EstaFinalizado())
                return Erro.Negocio("Não é possível remover retificações de uma lista de presença com situação 'Finalizado'.");

            await repositorioCodafRetificacaoListaPresenca.Remover(retificacao);
            return true;
        }
    }
}
