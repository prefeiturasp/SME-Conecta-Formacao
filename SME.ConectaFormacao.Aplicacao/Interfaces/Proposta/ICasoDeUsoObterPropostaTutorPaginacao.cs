using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaTutorPaginacao
    {
        Task<PaginacaoResultadoDto<PropostaTutorDTO>> Executar(long id);
    }
}