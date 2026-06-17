using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafSuplementarAnexo : EntidadeBaseAuditavel
    {
        public long CodafSuplementarId { get; set; }
        public virtual CodafSuplementar? CodafSuplementar { get; set; }
        public required Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Extensao { get; set; }
        public required TipoAnexoCodaf TipoAnexoId { get; set; }
    }
}