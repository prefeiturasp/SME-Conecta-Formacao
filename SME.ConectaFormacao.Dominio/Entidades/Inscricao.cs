using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Inscricao : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long UsuarioId { get; set; }
        public long? CargoId { get; set; }
        public long? FuncaoId { get; set; }
        public long? ArquivoId { get; set; }
        public SituacaoInscricao Situacao { get; set; }
    }
}
