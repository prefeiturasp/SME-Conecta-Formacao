namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Ue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long DreId { get; set; }
        public Dre Dre { get; set; } = null!;
        public required string CodigoUe { get; set; }
        public required string NomeEscola { get; set; }
        public string NomeEscolaCompleto => $"{SiglaTipoEscola} {NomeEscola}";
        public int TipoEscola { get; set; }
        public required string SiglaTipoEscola { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
