namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class FiltroAutocompletarNumeroHomologacaoDto
    {
        public string? TermoBusca { get; set; }
        public bool ComCodaf { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}
