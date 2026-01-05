using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class DadosListagemFormacaoComTurmaDTO
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public IEnumerable<DadosListagemFormacaoTurma> Turmas { get; set; }
        public IEnumerable<TipoInscricao> TiposInscricoes { get; set; }
    }
}