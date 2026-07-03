using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas
{
    public class FiltroListagemResultadoCodafListaPresencaDto
    {
        public string? NomeFormacao { get; set; }
        public string? CodigoFormacao { get; set; }
        public string? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafListaPresenca? Status { get; set; }
        public DateTime? DataEnvioDf { get; set; }
        public bool PerfilRestrito { get; set; } = false;
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}