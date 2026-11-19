using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafConferenciaAnexo : EntidadeBaseAuditavel
    {
        public long CodafConferenciaId { get; set; }
        public CodafConferencia? CodafConferencia { get; set; }
        public required Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Extensao { get; set; }
        public TipoAnexoCodaf TipoAnexoId { get; set; } = TipoAnexoCodaf.ListaPresenca;
    }
}
