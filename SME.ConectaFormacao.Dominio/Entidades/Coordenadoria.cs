namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Coordenadoria : EntidadeBaseAuditavel
    {
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public IEnumerable<AreaPromotora> AreasPromotoras { get; set; } = [];
    }
}
