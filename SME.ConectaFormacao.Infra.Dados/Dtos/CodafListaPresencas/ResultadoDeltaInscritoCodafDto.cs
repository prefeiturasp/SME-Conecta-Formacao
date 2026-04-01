using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas
{
    public class ResultadoDeltaInscritoCodafDto
    {
        public TipoDeltaInscritoCodaf TipoDelta { get; set; }
        public ResultadoInscritoTurmaCodafListaPresencaDto DadosInscrito { get; set; } = new();
    }
}
