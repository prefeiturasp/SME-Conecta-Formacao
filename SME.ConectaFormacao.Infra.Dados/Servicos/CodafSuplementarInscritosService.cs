using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class CodafSuplementarInscritosService(IRepositorioCodafSuplementarInscricao repositorioCodafInscritos) :
        ICodafSuplementarInscritosService
    {
        public async Task SalvarInscritosAsync(List<CodafSuplementarInscricao> inscritos, long codafSuplementarId)
        {
            await repositorioCodafInscritos.ExcluirPorCodafSuplementarIdAsync(codafSuplementarId);

            if (inscritos is not null && inscritos.Count != 0)
            {
                inscritos.ForEach(i => i.CodafSuplementarId = codafSuplementarId);
                await repositorioCodafInscritos.InserirVariosAsync(inscritos);
            }
        }
    }
}
