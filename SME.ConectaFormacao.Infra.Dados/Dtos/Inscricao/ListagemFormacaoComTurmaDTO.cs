namespace SME.ConectaFormacao.Infra
{
    public class ListagemFormacaoComTurmaDTO
    {
        public long? InscricaoId { get; set; }
        public long? PropostaId { get; set; }
        public int? QuantidadeVagas { get; set; }
        public string? NomeTurma { get; set; }
        public string? Datas { get; set; }
    }
}
