using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class CodafInscritosListaPresencaService(IRepositorioCodafInscritosListaPresenca repositorioCodafInscritos) :
        ICodafInscritosListaPresencaService
    {
        public async Task SalvarInscritosAsync(List<CodafInscricaoListaPresenca> inscritos, long codafListaPresencaId)
        {
            await repositorioCodafInscritos.ExcluirPorListaPresencaIdAsync(codafListaPresencaId);

            if (inscritos is not null && inscritos.Count != 0)
            {
                inscritos.ForEach(i => i.CodafListaPresencaId = codafListaPresencaId);
                await repositorioCodafInscritos.InserirVariosAsync(inscritos);
            }
        }
    }
}
