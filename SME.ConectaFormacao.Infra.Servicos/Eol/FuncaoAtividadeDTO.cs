namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class FuncaoAtividadeDto
    {
        public required string CdRegistroFuncional { get; set; }
        public required string CdTipoFuncao { get; set; }
        public required string CdDre { get; set; }
        public required string CdUe { get; set; }
        public string? NomeFuncao { get; set; }
        public DateTime? DataPosse { get; set; }
        public int? TipoVinculo { get; set; }
    }
}
