using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoRemoverCodafRetificacaoListaPresenca(IRepositorioCodafRetificacaoListaPresenca repositorioCodafRetificacaoListaPresenca) :
        ICasoDeUsoRemoverCodafRetificacaoListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafRetificacaoListaPresencaId)
        {
            var retificacao = await repositorioCodafRetificacaoListaPresenca.ObterPorId(codafRetificacaoListaPresencaId);
            if (retificacao is not null) await repositorioCodafRetificacaoListaPresenca.Remover(retificacao);
            return true;
        }
    }
}
