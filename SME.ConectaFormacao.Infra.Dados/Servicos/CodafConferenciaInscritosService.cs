using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class CodafConferenciaInscritosService(IRepositorioCodafConferenciaInscricao repositorioCodafInscritos) :
        ICodafConferenciaInscritosService
    {
        public async Task SalvarInscritosAsync(List<CodafConferenciaInscricao> inscritos, long codafConferenciaId)
        {
            await repositorioCodafInscritos.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId);

            if (inscritos is not null && inscritos.Count != 0)
            {
                inscritos.ForEach(i => i.CodafConferenciaId = codafConferenciaId);
                await repositorioCodafInscritos.InserirVariosAsync(inscritos);
            }
        }
    }
}
