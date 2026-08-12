namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoInscritoDto
    {
        public long Id { get; set; }
        public long InscricaoId { get; set; }
        public long CodafCursoNaoHomologadoId { get; set; }
        public bool Participou { get; set; }
    }
}