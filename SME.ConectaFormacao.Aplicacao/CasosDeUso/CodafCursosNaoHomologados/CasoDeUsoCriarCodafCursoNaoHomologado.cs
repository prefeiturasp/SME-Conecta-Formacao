using AutoMapper;
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
        IMapper mapper,
        ITransacao transacao) : ICasoDeUsoCriarCodafCursoNaoHomologado
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
                await SalvarInscritosAsync(codafCursoNaoHomologadoCadastroDto, idCodafCursoNaoHomologado);
                await SalvarAnexosAsync(codafCursoNaoHomologadoCadastroDto, idCodafCursoNaoHomologado);

                transacaoDb.Commit();
                return mapper.Map<CodafCursoNaoHomologadoDetalhadoDto>(codafCursoNaoHomologado);
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar o codaf.");
            }
        }

        private async Task SalvarInscritosAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, long codafCursoNaoHomologadoId)
        {
            var inscritos = mapper.Map<List<CodafCursoNaoHomologadoInscricao>>(codafCursoNaoHomologadoCadastroDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafCursoNaoHomologadoId);
        }

        private async Task SalvarAnexosAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, long codafCursoNaoHomologadoId)
        {
            var anexos = mapper.Map<List<CodafCursoNaoHomologadoAnexo>>(codafCursoNaoHomologadoCadastroDto.Anexos);
            await dependencias.AnexoService.ProcessarAnexosAsync(codafCursoNaoHomologadoId, anexos);
        }
    }
}
