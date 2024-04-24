using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarPropostaParecer
    {
        Task<long> Executar(PropostaParecerDTO propostaParecerDTO);
    }
}