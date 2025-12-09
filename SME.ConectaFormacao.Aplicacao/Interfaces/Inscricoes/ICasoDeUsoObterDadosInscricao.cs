using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterDadosInscricao
    {
        Task<DadosInscricaoDto> Executar();
    }
}
