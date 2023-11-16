using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarPropostaTutor
    {
        Task<long> Executar(long id, PropostaTutorDTO propostaTutorDto);
    }
}