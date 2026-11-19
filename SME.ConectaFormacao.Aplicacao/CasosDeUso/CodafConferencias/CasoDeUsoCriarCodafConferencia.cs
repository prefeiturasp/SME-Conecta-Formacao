using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoCriarCodafConferencia(
        CodafConferenciaDependencias dependencias) : CasoDeUsoCodafConferenciaBase(dependencias), ICasoDeUsoCriarCodafConferencia
    {
        public async Task<Resultado<CodafConferenciaDetalhadoDto>> ExecutarAsync(CodafConferenciaCadastroDto dto)
        {
            var proposta = await dependencias.RepositorioProposta.ObterNaoExcluidosPorIdAsync(dto.PropostaId);
            if (proposta is null)
                return Erro.NaoEncontrado("Proposta não encontrada.");

            if (proposta.TipoFormacao != TipoFormacao.Conferencia)
                return Erro.Negocio("A proposta informada não é do tipo Conferência.");

            if (proposta.Situacao != SituacaoProposta.Publicada)
                return Erro.Negocio("A proposta informada deve estar com a situação Publicada.");

            var jaPossuiCodaf = await dependencias.RepositorioCodaf.TurmaJaTemCodafAsync(dto.PropostaTurmaId);
            if (jaPossuiCodaf)
                return Erro.Negocio("Já existe um CODAF cadastrado para esta turma.");

            var dadosPublicacao = new DadosPublicacaoLista(
                dto.DataComunicado,
                dto.DataPublicacaoDom,
                dto.NumeroComunicado,
                dto.PaginaComunicadoDom,
                dto.CodigoCursoEol,
                dto.CodigoNivel,
                dto.Observacao);

            var codafConferencia = new CodafConferencia(
                dto.PropostaId,
                dto.PropostaTurmaId,
                dadosPublicacao);

            using var transacaoDb = dependencias.Transacao.Iniciar();
            try
            {
                var idCodafConferencia = await dependencias.RepositorioCodaf.Inserir(codafConferencia);
                codafConferencia.Id = idCodafConferencia;
                await SalvarDependenciasCodafAsync(dto, codafConferencia);

                transacaoDb.Commit();
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar o codaf.");
            }

            return new CodafConferenciaDetalhadoDto
            {
                Id = codafConferencia.Id,
                PropostaId = codafConferencia.PropostaId,
                PropostaTurmaId = codafConferencia.PropostaTurmaId,
                CodigoFormacao = codafConferencia.PropostaId,
                NumeroComunicado = codafConferencia.NumeroComunicado,
                DataComunicado = codafConferencia.DataComunicado,
                PaginaComunicadoDom = codafConferencia.PaginaComunicadoDom,
                DataPublicacaoDom = codafConferencia.DataPublicacaoDom,
                CodigoCursoEol = codafConferencia.CodigoCursoEol,
                CodigoNivel = codafConferencia.CodigoNivel,
                Observacao = codafConferencia.Observacao,
                Status = codafConferencia.Status,
                CriadoEm = codafConferencia.CriadoEm,
                CriadoLogin = codafConferencia.CriadoLogin,
                CriadoPor = codafConferencia.CriadoPor,
                AlteradoEm = codafConferencia.AlteradoEm,
                AlteradoLogin = codafConferencia.AlteradoLogin,
                AlteradoPor = codafConferencia.AlteradoPor
            };
        }
    }
}
