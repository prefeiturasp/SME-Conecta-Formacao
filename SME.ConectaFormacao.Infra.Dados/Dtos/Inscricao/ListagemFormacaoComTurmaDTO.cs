using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra
{
    public class ListagemFormacaoComTurmaDTO
    {
        public int TotalInscricoes { get; set; }
        public long? PropostaId { get; set; }
        public int? QuantidadeVagas { get; set; }
        public string? NomeTurma { get; set; }
        public string? Datas { get; set; }
        public SituacaoInscricao Situacao { get; set; }
    }
}
