using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoFinalizarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoFinalizarCodafListaPresenca
    {
        public async Task<Resultado> ExecutarAsync(long codafListaPresencaId)
        {
            var perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var codafListaPresenca = await repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(codafListaPresencaId);

            if (codafListaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (perfilRestrito && codafListaPresenca.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para finalizar esta lista de presença.");

            if (codafListaPresenca.EstaFinalizado())
                return Erro.Negocio("Não é possível finalizar uma lista de presença com a situação 'Finalizada'.");

            if (!codafListaPresenca.PodeSerExcluido(contextoAplicacao.IdPerfilUsuario))
                return Erro.Negocio("Essa lista não pode ser finalizada.");

            if (codafListaPresenca.CodafInscricoes.Count > 0 && codafListaPresenca.CodafInscricoes.Any(i => i.Aprovado == true))
                return Erro.NaoEncontrado("Lista de presença só pode ser finalizada se não houver aprovações.");

            await repositorioCodafListaPresenca.FinalizarAsync(codafListaPresencaId);
            return Resultado.DeSucesso();
        }
    }
}
