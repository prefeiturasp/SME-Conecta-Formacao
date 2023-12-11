using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostaTutorPorId
    {
        Task<PropostaTutorDTO> Executar(long tutorId);
    }
}