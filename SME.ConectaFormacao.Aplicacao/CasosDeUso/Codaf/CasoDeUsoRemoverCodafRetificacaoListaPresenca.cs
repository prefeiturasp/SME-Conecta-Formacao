using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoRemoverCodafRetificacaoListaPresenca(IRepositorioCodafRetificacaoListaPresenca repositorioCodafRetificacaoListaPresenca, IRepositorioCodafListaPresenca repositorioCodafListaPresenca, IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoRemoverCodafRetificacaoListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long codafRetificacaoListaPresencaId)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var retificacao = await repositorioCodafRetificacaoListaPresenca.ObterNaoExcluidosPorIdAsync(codafRetificacaoListaPresencaId);

            if (retificacao is null)
                return Erro.NaoEncontrado("Retificação não encontrada.");

            var codaf = await repositorioCodafListaPresenca.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId);

            if (codaf is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (perfilRestrito && codaf.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Não é possível remover retificações de um CODAF criado por outro usuário.");

            if (codaf.EstaFinalizado())
                return Erro.Negocio("Não é possível remover retificações de uma lista de presença com situação 'Finalizado'.");

            await repositorioCodafRetificacaoListaPresenca.Remover(retificacao);
            return true;
        }
    }
}
