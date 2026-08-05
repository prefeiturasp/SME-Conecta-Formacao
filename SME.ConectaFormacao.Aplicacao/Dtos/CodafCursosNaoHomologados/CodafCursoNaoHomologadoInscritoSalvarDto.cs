namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados
{
    public class CodafCursoNaoHomologadoInscritoSalvarDto
    {
        public required long InscricaoId { get; set; }
        public required bool Participou { get; set; }
    }
}