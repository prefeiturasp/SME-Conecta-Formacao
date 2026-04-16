
namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class RetornoTurmaDetalheDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string[] Periodos { get; set; }
        public string Local { get; set; }
        public string Horario { get; set; }
        public bool InscricaoEncerrada { get; set; }
        public List<string>? DatasEncontros { get; set; }
        public List<DataEncontroNovoDto>? DataEncontrosNovo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
