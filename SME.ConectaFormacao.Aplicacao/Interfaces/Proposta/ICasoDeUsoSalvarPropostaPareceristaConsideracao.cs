using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarPropostaPareceristaConsideracao
    {
        Task<RetornoDTO> Executar(PropostaPareceristaConsideracaoCadastroDTO propostaPareceristaConsideracaoCadastroDto);
    }
}