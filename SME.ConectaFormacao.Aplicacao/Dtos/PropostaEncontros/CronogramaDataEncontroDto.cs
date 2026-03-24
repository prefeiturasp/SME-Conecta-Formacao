namespace SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros
{
    public readonly record struct CronogramaDataEncontroDto(
        DateTime Data, 
        string? HoraInicio, 
        string? HoraFim);
}
