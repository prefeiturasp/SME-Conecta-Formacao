using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAtribuirPropostaAoGrupoGestao
    {
        Task<bool> Executar(long propostaId, string justificativa);
    }
}