namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes
{
    public class CargoFuncaoUsuarioDto
    {
        public string? Codigo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string UeCodigo { get; set; } = string.Empty;
        public string DreCodigo { get; set; } = string.Empty;
        public int TipoVinculo { get; set; }
        public DateTime? DataInicio { get; set; }
    }
}
