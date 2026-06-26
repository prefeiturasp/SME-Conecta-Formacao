using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoSalvarLog
    {
        Task<bool> Executar(LogDto logDto);
    }
}