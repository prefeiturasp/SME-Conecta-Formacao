namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoInscritoDto
    {
        public long Id { get; set; }
        public long InscricaoId { get; set; }
        public long CodafCursoNaoHomologadoId { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public bool Participou { get; set; }
    }
}