using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos
{
    public record CargoFuncaoEolDto(int Codigo, TipoCargoFuncao TipoCargoFuncao, string? Nome, DateOnly? DataPosse, int? TipoVinculo);
}
