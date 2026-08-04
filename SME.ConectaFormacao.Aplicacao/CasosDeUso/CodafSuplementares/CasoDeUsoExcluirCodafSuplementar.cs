using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoExcluirCodafSuplementar(
        IRepositorioCodafSuplementar repositorioCodafSuplementar) : ICasoDeUsoExcluirCodafSuplementar
    {
        public async Task<Resultado> ExecutarAsync(long codafSuplementarId)
        {
            var codafSuplementar = await repositorioCodafSuplementar.ObterNaoExcluidosPorIdAsync(codafSuplementarId);
            if (codafSuplementar == null)
                return Erro.NaoEncontrado("Codaf suplementar não encontrado.");

            if (codafSuplementar.EstaFinalizado())
                return Erro.Negocio("Não é possível excluir um Codaf suplementar com situação 'Finalizado'.");

            await repositorioCodafSuplementar.ExcluirAsync(codafSuplementarId);
            return Resultado.DeSucesso();
        }
    }
}
