using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IValidadorCodafListaPresencaService
    {
        Task<Erro?> ValidarVinculoPropostaTurmaAsync(long propostaId, long propostaTurmaId);
        Task<Erro?> ValidarUnicidadeTurmaListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0);
    }
}
