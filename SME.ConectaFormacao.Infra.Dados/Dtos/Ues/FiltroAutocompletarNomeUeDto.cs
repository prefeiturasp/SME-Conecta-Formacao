namespace SME.ConectaFormacao.Infra.Dados.Dtos.Ues
{
    public class FiltroAutocompletarNomeUeDto
    {
        public string? TermoBusca { get; set; }
        public long DreId { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}
