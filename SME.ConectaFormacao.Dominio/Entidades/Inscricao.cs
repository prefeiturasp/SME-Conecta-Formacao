using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Inscricao : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long UsuarioId { get; set; }
        public long? CodigoCargoEol { get; set; }
        public string? CargoEol { get; set; }
        public long? CodigoTipoFuncaoEol { get; set; }
        public string? TipoFuncaoEol { get; set; }
        public long? ArquivoId { get; set; }
        public SituacaoInscricao Situacao { get; set; }
    }
}
