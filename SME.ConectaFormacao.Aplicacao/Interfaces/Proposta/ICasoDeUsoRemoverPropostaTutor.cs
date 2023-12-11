namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRemoverPropostaTutor
    {
        Task<bool> Executar(long tutorId);
    }
}