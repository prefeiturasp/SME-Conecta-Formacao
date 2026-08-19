namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class PropostaTurmaDto
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Descricao => Nome + (DataFim != null 
                                          ? $" {DataInicio:dd/MM/yyyy} até {DataFim:dd/MM/yyyy}" 
                                          : $" {DataInicio:dd/MM/yyyy}");
    }
}
