#pragma warning disable CS8618
namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class TurmaRelatorioCodafDto
    {
        public string NomeTurma { get; set; } // Nome da Aba (Worksheet)

        // Blocos de Dados
        public CabecalhoRelatorioCodafDto Cabecalho { get; set; }
        public List<RegenteTurmaRelatorioCodafDto> RegentesDaTurma { get; set; }

        // Listas segmentadas para os blocos 4 e 5
        public GrupoAlunosRelatorioCodafDto AlunosAprovadosMunicipal { get; set; }
        public GrupoAlunosRelatorioCodafDto AlunosAprovadosParceira { get; set; }
        public GrupoAlunosRelatorioCodafDto AlunosReprovadosMunicipal { get; set; }
        public GrupoAlunosRelatorioCodafDto AlunosReprovadosParceira { get; set; }
    }
}

