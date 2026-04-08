namespace SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros
{
    public readonly record struct CronogramaDataEncontroDto(
        long Id,
        DateTime Data, 
        string? HoraInicio, 
        string? HoraFim);
}
