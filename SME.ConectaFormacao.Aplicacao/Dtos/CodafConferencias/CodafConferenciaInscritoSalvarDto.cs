namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias
{
    public class CodafConferenciaInscritoSalvarDto
    {
        public required long InscricaoId { get; set; }
        public bool? Participou { get; set; }
    }
}
