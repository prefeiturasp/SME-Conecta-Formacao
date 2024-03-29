namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemFormacaoComTurmaDTO
    {
        public DadosListagemFormacaoComTurmaDTO()
        {
            Turmas = new List<DadosListagemFormacaoTurma>();
        }
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public IEnumerable<DadosListagemFormacaoTurma> Turmas { get; set; }
    }

}