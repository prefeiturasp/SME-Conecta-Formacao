using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoAtualizarCodafConferencia(
        CodafConferenciaDependencias dependencias,
        IContextoAplicacao contextoAplicacao) : CasoDeUsoCodafConferenciaBase(dependencias), ICasoDeUsoAtualizarCodafConferencia
    {
        public async Task<Resultado> ExecutarAsync(CodafConferenciaCadastroDto dto, long id)
        {
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafConferenciaExistente = await dependencias.RepositorioCodaf.ObterNaoExcluidosPorIdAsync(id);
            if (codafConferenciaExistente is null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            if (perfilRestrito && codafConferenciaExistente.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para editar este Codaf.");

            if (codafConferenciaExistente.EstaFinalizado())
                return Erro.Negocio("Não é possível editar um Codaf com status 'Finalizado'.");

            if (codafConferenciaExistente.CertificadoEmitido)
                return Erro.Negocio("Não é possível editar um Codaf com certificados emitidos.");

            var dadosPublicacao = new DadosPublicacaoLista(
                dto.DataComunicado,
                dto.DataPublicacaoDom,
                dto.NumeroComunicado,
                dto.PaginaComunicadoDom,
                dto.CodigoCursoEol,
                dto.CodigoNivel,
                dto.Observacao);

            codafConferenciaExistente.AtualizarInformacoes(dadosPublicacao);

            using var transacaoDb = dependencias.Transacao.Iniciar();
            try
            {
                await SalvarDependenciasCodafAsync(dto, codafConferenciaExistente);
                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar CODAF");
            }
        }
    }
}
