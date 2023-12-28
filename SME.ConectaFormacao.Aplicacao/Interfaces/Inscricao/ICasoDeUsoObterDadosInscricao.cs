using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterDadosInscricao
    {
        Task<DadosInscricaoDTO> Executar();
    }
}
