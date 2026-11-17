using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias
{
    public class FiltroListagemResultadoCodafConferenciaDto
    {
        public string? NomeFormacao { get; set; }
        public string? CodigoFormacao { get; set; }
        public string? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafConferencia? Status { get; set; }
        public DateTime? DataCriacao { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}
