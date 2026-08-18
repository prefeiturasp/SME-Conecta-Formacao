using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares
{
    public interface ICasoDeUsoFinalizarCodafSuplementar
    {
        Task<Resultado> ExecutarAsync(long codafSuplementarId);
    }
}
