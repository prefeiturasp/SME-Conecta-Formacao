using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados
{
    public class FiltroListagemResultadoCodafCursoNaoHomologadoDto
    {
        public string? NomeFormacao { get; set; }
        public string? CodigoFormacao { get; set; }
        public string? NumeroHomologacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public StatusCodafCursoNaoHomologado? Status { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}