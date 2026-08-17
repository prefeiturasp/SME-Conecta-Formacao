using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoExcluirCodafCursoNaoHomologado(
        IRepositorioCodafCursoNaoHomologado repositorioCodaf,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoExcluirCodafCursoNaoHomologado
    {
        public async Task<Resultado> ExecutarAsync(long codafCursoNaoHomologadoId)
        {
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafCursoNaoHomologado = await repositorioCodaf.ObterNaoExcluidosPorIdAsync(codafCursoNaoHomologadoId);

            if (codafCursoNaoHomologado is null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            if (codafCursoNaoHomologado.EstaFinalizado())
                return Erro.Negocio("Codaf finalizado não pode ser excluído.");

            if (perfilRestrito && codafCursoNaoHomologado.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para excluir este Codaf.");

            await repositorioCodaf.ExcluirAsync(codafCursoNaoHomologadoId);
            return Resultado.DeSucesso();
        }
    }
}
