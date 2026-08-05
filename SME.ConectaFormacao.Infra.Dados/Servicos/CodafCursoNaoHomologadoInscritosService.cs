using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class CodafCursoNaoHomologadoInscritosService(IRepositorioCodafCursoNaoHomologadoInscricao repositorioCodafInscritos) :
        ICodafCursoNaoHomologadoInscritosService
    {
        public async Task SalvarInscritosAsync(List<CodafCursoNaoHomologadoInscricao> inscritos, long codafCursoNaoHomologadoId)
        {
            await repositorioCodafInscritos.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId);

            if (inscritos is not null && inscritos.Count != 0)
            {
                inscritos.ForEach(i => i.CodafCursoNaoHomologadoId = codafCursoNaoHomologadoId);
                await repositorioCodafInscritos.InserirVariosAsync(inscritos);
            }
        }
    }
}
