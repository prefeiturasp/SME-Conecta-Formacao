using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias
{
    public class FiltroCodafConferenciaDto
    {
        public string? NomeFormacao { get; set; }
        public long? CodigoFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafConferencia? Status { get; set; }
        public DateTime? DataCriacao { get; set; }
        public int NumeroPagina { get; set; } = 1;
        public int NumeroRegistros { get; set; } = 10;
    }
}
