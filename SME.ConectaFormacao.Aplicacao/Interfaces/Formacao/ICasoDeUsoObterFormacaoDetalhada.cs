using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Formacao
{
    public interface ICasoDeUsoObterFormacaoDetalhada
    {
        Task<RetornoFormacaoDetalhadaDTO> Executar(long propostaId);
    }
}
