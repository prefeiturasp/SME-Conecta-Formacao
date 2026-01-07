using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IValidadorCodafListaPresencaService
    {
        Task<Erro?> ValidarVinculoPropostaTurmaAsync(long propostaId, long propostaTurmaId);
        Task<Erro?> ValidarUnicidadeTurmaListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0);
        Task<Erro?> ValidarParaEnvioAoDfAsync(CodafListaPresenca codafListaPresenca);
    }
}
