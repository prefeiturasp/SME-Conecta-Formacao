using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoExcluirCodafSuplementar
    {
        Task<Resultado> ExecutarAsync(long codafSuplementarId);
    }
}
