#pragma warning disable CS8618
namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class DadosParticipanteRelatorioCodafDto
    {
        public string Documento { get; set; }
        public bool TemRf { get; set; }
        public bool Aprovado { get; set; }
        public bool AtividadeObrigatoria { get; set; }
        public string ConceitoFinal { get; set; }
        public decimal PercentualFrequencia { get; set; }
        public long CodigoCertificado { get; set; }
        public string Nome { get; set; }
    }
}

