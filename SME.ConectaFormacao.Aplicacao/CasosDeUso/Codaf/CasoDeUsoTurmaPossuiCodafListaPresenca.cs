using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoTurmaPossuiCodafListaPresenca(IRepositorioCodafListaPresenca repositorioCodafListaPresenca) : ICasoDeUsoTurmaPossuiCodafListaPresenca
    {
        public async Task<Resultado<bool>> ExecutarAsync(long propostaTurmaId, long listaPresencaId = 0)
        {
            var possuiListaPresenca = await repositorioCodafListaPresenca.TurmaJaTemListaDePresencaAsync(propostaTurmaId, listaPresencaId);
            return possuiListaPresenca;
        }
    }
}