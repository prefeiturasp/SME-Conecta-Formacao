namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados
{
    public class DadosParticipanteRelatorioCodafCursoNaoHomologadoDto
    {
        public string Documento { get; set; } = string.Empty;
        public bool TemRf { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Participou { get; set; }
        public long? CodigoCertificadoDeclaracao { get; set; }
    }
}