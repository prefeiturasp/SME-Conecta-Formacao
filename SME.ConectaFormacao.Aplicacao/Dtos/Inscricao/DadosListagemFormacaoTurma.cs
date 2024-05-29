namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoTurma
    {
        public string? NomeTurma { get; set; }
        public string? Data { get; set; }
        public int? QuantidadeVagas { get; set; }
        public int? QuantidadeInscricoes { get; set; }
        public int? Confirmadas { get; set; }
        public bool PodeRealizarSorteio { get; set; }
        public int? AguardandoAnalise { get; set; }
        public int? EmEspera { get; set; }
        public int? Cancelada { get; set; }
    }
}