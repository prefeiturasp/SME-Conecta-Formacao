namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoTurma
    {
        public string? NomeTurma { get; set; }
        public string Data { get; set; }
        public long QuantidadeVagas { get; set; }
        public long QuantidadeInscricoes { get; set; }
    }
}