using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoExcluirCodafConferencia(
        IRepositorioCodafConferencia repositorioCodaf,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoExcluirCodafConferencia
    {
        public async Task<Resultado> ExecutarAsync(long codafConferenciaId)
        {
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafConferencia = await repositorioCodaf.ObterNaoExcluidosPorIdAsync(codafConferenciaId);
            if (codafConferencia is null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            if (!codafConferencia.PodeSerExcluido(contextoAplicacao.IdPerfilUsuario))
                return Erro.Negocio("Apenas registros com situação 'Iniciado' podem ser excluídos.");

            if (perfilRestrito && codafConferencia.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para excluir este Codaf.");

            await repositorioCodaf.ExcluirAsync(codafConferenciaId);
            return Resultado.DeSucesso();
        }
    }
}
