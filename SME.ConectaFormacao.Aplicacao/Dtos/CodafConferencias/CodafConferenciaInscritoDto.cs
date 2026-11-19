namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias
{
    public class CodafConferenciaInscritoDto
    {
        public long Id { get; set; }
        public long InscricaoId { get; set; }
        public long CodafConferenciaId { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public bool? Participou { get; set; }
    }
}
