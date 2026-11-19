using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoExcluirCodafConferencia
    {
        Task<Resultado> ExecutarAsync(long codafConferenciaId);
    }
}
