using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class FiltroListaPresencaCodafDto
    {
        public string? NomeFormacao { get; set; }
        public long? CodigoFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafListaPresenca? Status { get; set; }
        public DateTime? DataEnvioDf { get; set; }
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}
