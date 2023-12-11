using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterCriterioValidacaoInscricao
    {
        Task<IEnumerable<CriterioValidacaoInscricaoDTO>> Executar(bool exibirOpcaoOutros);
    }
}
