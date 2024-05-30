namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoTurma
    {
        public string? NomeTurma { get; set; }
        public string? Data { get; set; }
        public int? QuantidadeVagas { get; set; }
        public int? QuantidadeInscricoes { get; set; }
        public int? QuantidadeConfirmada { get; set; }
        public int? QuantidadeAguardandoAnalise { get; set; }
        public int? QuantidadeEmEspera { get; set; }
        public int? QuantidadeCancelada { get; set; }
        public int? QuantidadeDisponivel { get; set; }
        public int? QuantidadeExcedida { get; set; }
        public DadosListagemFormacaoTurmaPermissao Permissao { get; set; }
    }

    public class DadosListagemFormacaoTurmaPermissao
    {
        public bool PodeRealizarSorteio { get; set; }
    }
}