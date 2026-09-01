using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoCriarCodafCursoNaoHomologado(
        CodafCursoNaoHomologadoDependencias dependencias,
        ITransacao transacao) : CasoDeUsoCodafCursoNaoHomologadoBase(dependencias), ICasoDeUsoCriarCodafCursoNaoHomologado
    {
        public async Task<Resultado<CodafCursoNaoHomologadoDetalhadoDto>> ExecutarAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto)
        {
            var codafCursoNaoHomologado = new CodafCursoNaoHomologado(
                codafCursoNaoHomologadoCadastroDto.PropostaId,
                codafCursoNaoHomologadoCadastroDto.PropostaTurmaId, codafCursoNaoHomologadoCadastroDto.Observacao);

            using var transacaoDb = transacao.Iniciar();
            try
            {
                var idCodafCursoNaoHomologado = await dependencias.RepositorioCodaf.Inserir(codafCursoNaoHomologado);
                codafCursoNaoHomologado.Id = idCodafCursoNaoHomologado;
                await SalvarDependenciasCodafAsync(codafCursoNaoHomologadoCadastroDto, codafCursoNaoHomologado);

                transacaoDb.Commit();
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar o codaf.");
            }

            return new CodafCursoNaoHomologadoDetalhadoDto
            { 
                AlteradoEm = codafCursoNaoHomologado.AlteradoEm,
                AlteradoLogin = codafCursoNaoHomologado.AlteradoLogin,
                AlteradoPor = codafCursoNaoHomologado.AlteradoPor,
                CodigoFormacao = codafCursoNaoHomologado.PropostaId,
                CriadoEm = codafCursoNaoHomologado.CriadoEm,
                CriadoLogin = codafCursoNaoHomologado.CriadoLogin,
                CriadoPor = codafCursoNaoHomologado.CriadoPor,
                Id = codafCursoNaoHomologado.Id,
                PropostaId = codafCursoNaoHomologado.PropostaId,
                PropostaTurmaId = codafCursoNaoHomologado.PropostaTurmaId,
                Observacao = codafCursoNaoHomologado.Observacao,
                Status = codafCursoNaoHomologado.Status
            };
        }
    }
}
