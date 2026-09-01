using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public abstract class CasoDeUsoCodafCursoNaoHomologadoBase(CodafCursoNaoHomologadoDependencias dependencias)
    {
        protected readonly CodafCursoNaoHomologadoDependencias dependencias = dependencias;

        protected async Task SalvarDependenciasCodafAsync(CodafCursoNaoHomologadoCadastroDto dto, CodafCursoNaoHomologado entidade)
        {
            await SalvarInscritosAsync(dto, entidade);
            await SalvarAnexosAsync(dto, entidade);
            entidade.DefinirStatus();
            await dependencias.RepositorioCodaf.Atualizar(entidade);
        }

        private async Task SalvarAnexosAsync(CodafCursoNaoHomologadoCadastroDto dto, CodafCursoNaoHomologado entidade)
        {
            var anexos = dependencias.Mapper.Map<List<CodafCursoNaoHomologadoAnexo>>(dto.Anexos);
            await dependencias.AnexoService.ProcessarAnexosAsync(entidade.Id, anexos);
            entidade.CodafAnexos = anexos;
        }

        private async Task SalvarInscritosAsync(CodafCursoNaoHomologadoCadastroDto dto, CodafCursoNaoHomologado entidade)
        {
            var inscritos = dependencias.Mapper.Map<List<CodafCursoNaoHomologadoInscricao>>(dto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, entidade.Id);
            entidade.CodafInscricoes = inscritos;
        }
    }
}