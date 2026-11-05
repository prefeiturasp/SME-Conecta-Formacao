namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class PropostaTurmaDto
    {
        private string? _descricao;

        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool PossuiCodafListaPresenca { get; set; }

        public string Descricao
        {
            get => _descricao ?? (DataInicio != null || DataFim != null
                ? Nome + (DataFim != null
                    ? $" {DataInicio:dd/MM/yyyy} até {DataFim:dd/MM/yyyy}"
                    : $" {DataInicio:dd/MM/yyyy}")
                : Nome);
            set => _descricao = value;
        }
    }
}
