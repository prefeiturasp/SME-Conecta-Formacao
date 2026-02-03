using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafMovimentacaoListaPresenca
    {
        public long Id { get; set; }
        public long CodafListaPresencaId { get; set; }
        public StatusCodafListaPresenca StatusCodafListaPresenca { get; set; }
        public long? CodafComentarioListaPresencaId { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoLogin { get; set; } = null!;
        public string CriadoPor { get; set; } = null!;
    }
}
