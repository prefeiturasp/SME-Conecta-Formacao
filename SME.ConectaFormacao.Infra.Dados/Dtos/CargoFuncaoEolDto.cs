using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos
{
    public record CargoFuncaoEolDto
    {
        public int Codigo { get; init; }
        public int CargoFuncaoId { get; init; }
        public TipoCargoFuncao TipoCargoFuncao { get; init; }
        public string? Nome { get; init; }
        public DateTime? DataPosse { get; init; }
        public int? TipoVinculo { get; init; }
        public string CodigoDre { get; init; } = string.Empty;
        public string CodigoUe { get; init; } = string.Empty;
        public List<FuncaoDoCargoEolDto> Funcoes { get; set; } = [];
    }

    public record FuncaoDoCargoEolDto
    {
        public int CodigoFuncao { get; init; }
        public int CargoFuncaoId { get; init; }
        public string? Nome { get; init; }

        public DateTime? DataPosse { get; init; }

        public int? TipoVinculo { get; init; }
        public string CodigoDre { get; init; } = string.Empty;
        public string CodigoUe { get; init; } = string.Empty;
    }
}