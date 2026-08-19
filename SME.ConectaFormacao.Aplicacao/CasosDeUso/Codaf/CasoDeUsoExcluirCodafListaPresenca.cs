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
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafListaPresenca = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(codafListaPresencaId);

            if (codafListaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (perfilRestrito && codafListaPresenca.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para excluir esta lista de presença.");

            if (codafListaPresenca.EstaFinalizado())
                return Erro.Negocio("Não é possível excluir uma lista de presença com situação 'Finalizado'.");

            if (!codafListaPresenca.PodeSerExcluido(contextoAplicacao.IdPerfilUsuario))
                return Erro.Negocio("Essa lista não pode ser excluída.");

            await repositorioCodafListaPresenca.ExcluirAsync(codafListaPresencaId);
            return Resultado.DeSucesso();
        }
    }
}
