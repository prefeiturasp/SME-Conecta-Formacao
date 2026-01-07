using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafAnexo : EntidadeBaseAuditavel
    {
        public long CodafListaPresencaId { get; set; }
        public virtual CodafListaPresenca? CodafListaPresenca { get; set; }
        public required Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Extensao { get; set; }
        public required TipoAnexoCodaf TipoAnexoId { get; set; }
    }
}