using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoRemoverCodafSuplementarRetificacao(
        IRepositorioCodafSuplementarRetificacao repositorioCodafSuplementarRetificacao,
        IRepositorioCodafSuplementar repositorioCodafSuplementar) :
        ICasoDeUsoRemoverCodafSuplementarRetificacao
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafSuplementarRetificacaoId)
        {
            var retificacao = await repositorioCodafSuplementarRetificacao.ObterNaoExcluidosPorIdAsync(codafSuplementarRetificacaoId);
            if (retificacao is null)
                return Erro.NaoEncontrado("Retificação não encontrada.");

            var codaf = await repositorioCodafSuplementar.ObterNaoExcluidosPorIdAsync(retificacao.CodafSuplementarId);
            if (codaf is null)
                return Erro.NaoEncontrado("Codaf suplementar não encontrada.");

            await repositorioCodafSuplementarRetificacao.Remover(retificacao);
            return true;
        }
    }
}
