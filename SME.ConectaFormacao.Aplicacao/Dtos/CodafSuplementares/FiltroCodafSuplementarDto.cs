using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class FiltroCodafSuplementarDto
    {
        public string? NomeFormacao { get; set; }
        public long? CodigoFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafListaPresenca? Status { get; set; }
        public DateTime? DataEnvioDf { get; set; }
        public TipoCodafSuplementar Tipo { get; set; } = TipoCodafSuplementar.Homologado;
        public required int NumeroPagina { get; set; } = 1;
        public required int NumeroRegistros { get; set; } = 10;
    }
}
