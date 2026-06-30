using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoRemoverCodafSuplementarRetificacao
    {
        Task<Resultado<bool>> ExecutarAsync(long codafSuplementarRetificacaoId);
    }
}
