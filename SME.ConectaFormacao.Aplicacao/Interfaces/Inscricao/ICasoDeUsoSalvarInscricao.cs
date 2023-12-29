using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoSalvarInscricao
    {
        Task<long> Executar(InscricaoDTO inscricaoDTO);
    }
}
