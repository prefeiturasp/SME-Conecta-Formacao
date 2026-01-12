namespace SME.ConectaFormacao.Infra.Dados.Dtos
{
    public class FiltroListaFormacaoPropostaDto
    {
        public long[]? PublicosAlvosIds { get; set; }
        public string? Titulo { get; set; }
        public long[]? AreasPromotorasIds { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int[]? FormatosIds { get; set; }
        public long[]? PalavrasChavesIds { get; set; }
        public string? RfServidor { get; set; }
        public required bool FiltrarPorPerfil { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}