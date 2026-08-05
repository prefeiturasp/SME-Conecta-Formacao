namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoInscritoTurmaDto
    {
        public long Id { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public bool Participou { get; set; }
    }
}