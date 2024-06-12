using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoSortearInscricoes
    {
        Task<RetornoDTO> Executar(long propostaTurmaId);
    }
}
