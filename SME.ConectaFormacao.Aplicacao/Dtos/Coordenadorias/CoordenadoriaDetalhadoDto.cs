using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias
{
    public class CoordenadoriaDetalhadoDto
    {
        public long Id { get; set; }
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public IEnumerable<AreaPromotoraSimplificadoDto> AreasPromotoras { get; set; } = [];
    }
}