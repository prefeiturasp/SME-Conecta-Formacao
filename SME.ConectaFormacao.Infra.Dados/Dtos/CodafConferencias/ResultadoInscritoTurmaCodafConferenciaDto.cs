namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias
{
    public class ResultadoInscritoTurmaCodafConferenciaDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string? NomeSocial { get; set; }
        public string NomeExibicao => string.IsNullOrWhiteSpace(NomeSocial) ? Nome : NomeSocial;
        public bool? Participou { get; set; }
    }
}
