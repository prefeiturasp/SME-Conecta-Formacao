namespace SME.ConectaFormacao.Infra
{
    public class ListagemFormacaoComTurmaDTO
    {
        public int TotalInscricoes { get; set; }
        public long? PropostaId { get; set; }
        public long? PropostaTurmaId { get; set; }
        public int? QuantidadeVagas { get; set; }
        public string? NomeTurma { get; set; }
        public string? Datas { get; set; }
        public int? Confirmadas { get; set; }
        public int? AguardandoAnalise { get; set; }
        public int? EmEspera { get; set; }
        public int? Cancelada { get; set; }
        public int? Disponiveis { get; set; }
        public int? Excedidas { get; set; }
        public bool? PermiteSorteio { get; set; }
    }
}
