namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoComTurma
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public DadosListagemFormacaoTurma Turmas { get; set; }
    }
    
}