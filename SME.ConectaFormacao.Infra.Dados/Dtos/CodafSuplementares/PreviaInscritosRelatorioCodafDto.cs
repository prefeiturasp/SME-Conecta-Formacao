#pragma warning disable CS8618
namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class PreviaInscritosRelatorioCodafDto
    {
        public bool TemRf { get; set; }
        public int TotalInscritos { get; set; }
        public int TotalAprovados { get; set; }
        public int TotalReprovados { get; set; }
    }
}

