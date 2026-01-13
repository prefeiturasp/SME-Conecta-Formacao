using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoExcluirCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoExcluirCodafListaPresenca
    {
        public async Task<Resultado> ExecutarAsync(long codafListaPresencaId)
        {
            var codafListaPresenca = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(codafListaPresencaId);
            if (codafListaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (!codafListaPresenca.PodeSerExcluido(contextoAplicacao.IdPerfilUsuario))
                return Erro.Negocio("Essa lista não pode ser excluída.");

            await repositorioCodafListaPresenca.ExcluirAsync(codafListaPresencaId);
            return Resultado.DeSucesso();
        }
    }
}
