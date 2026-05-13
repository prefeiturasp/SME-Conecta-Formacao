namespace SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias
{
    public class CoordenadoriaFiltroDto
    {
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}