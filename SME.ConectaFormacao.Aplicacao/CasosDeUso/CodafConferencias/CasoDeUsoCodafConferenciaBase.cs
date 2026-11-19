using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public abstract class CasoDeUsoCodafConferenciaBase(CodafConferenciaDependencias dependencias)
    {
        protected readonly CodafConferenciaDependencias dependencias = dependencias;

        protected async Task SalvarDependenciasCodafAsync(CodafConferenciaCadastroDto dto, CodafConferencia entidade)
        {
            await SalvarInscritosAsync(dto, entidade);
            await SalvarAnexosAsync(dto, entidade);
            entidade.DefinirStatus();
            await dependencias.RepositorioCodaf.Atualizar(entidade);
        }

        private async Task SalvarAnexosAsync(CodafConferenciaCadastroDto dto, CodafConferencia entidade)
        {
            var anexos = dependencias.Mapper.Map<List<CodafConferenciaAnexo>>(dto.Anexos);
            await dependencias.AnexoService.ProcessarAnexosAsync(entidade.Id, anexos);
            entidade.CodafAnexos = anexos;
        }

        private async Task SalvarInscritosAsync(CodafConferenciaCadastroDto dto, CodafConferencia entidade)
        {
            var inscritos = dependencias.Mapper.Map<List<CodafConferenciaInscricao>>(dto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, entidade.Id);
            entidade.CodafInscricoes = inscritos;
        }
    }
}
