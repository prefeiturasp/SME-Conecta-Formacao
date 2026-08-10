namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class PropostaComTurmasDto
    {
        public long Id { get; set; }
        public string NomeFormacao { get; set; } = string.Empty;
        public long? NumeroHomologacao { get; set; }
        public ICollection<PropostaTurmaDto> Turmas { get; set; } = [];
    }
}
