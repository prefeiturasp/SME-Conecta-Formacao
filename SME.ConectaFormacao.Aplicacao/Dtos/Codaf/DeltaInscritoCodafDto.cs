namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public record DeltaInscritoCodafDto
    {
        public int TotalNovos => InscritosNovos.Count;
        public int TotalRemovidos => InscritosRemovidos.Count;
        public bool HouveAlteracao => TotalNovos > 0 || TotalRemovidos > 0;
        public IList<InscritoCodafResumidoDto> InscritosRemovidos { get; init; } = [];
        public IList<CodafInscritoTurmaListaPresencaRetornoDto> InscritosNovos { get; init; } = [];
    }
}