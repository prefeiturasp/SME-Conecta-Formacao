using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoFinalizarCodafCursoNaoHomologado(
        IRepositorioCodafCursoNaoHomologado repositorioCodafCursoNaoHomologado,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoFinalizarCodafCursoNaoHomologado
    {
        public async Task<Resultado> ExecutarAsync(long codafCursoNaoHomologadoId, FinalizarCodafDto dto)
        {
            if (dto == null || !dto.ConfirmacaoCiencia)
                return Erro.Negocio("É necessário confirmar a ciência dos dados para finalizar o CODAF.");

            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codaf = await repositorioCodafCursoNaoHomologado.ObterPorIdDetalhadoAsync(codafCursoNaoHomologadoId);

            if (codaf == null)
                return Erro.NaoEncontrado("CODAF não homologado não encontrado.");

            if (perfilRestrito && codaf.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para finalizar este CODAF.");

            if (codaf.EstaFinalizado())
                return Erro.Negocio("Não é possível finalizar um CODAF com a situação 'Finalizada'.");

            codaf.Finalizar();

            if (!codaf.EstaFinalizado())
                return Erro.Negocio("Não foi possível finalizar o CODAF.");

            await repositorioCodafCursoNaoHomologado.Atualizar(codaf);

            return Resultado.DeSucesso();
        }
    }
}
