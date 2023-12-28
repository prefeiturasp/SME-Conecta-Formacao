using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Inscricao : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long UsuarioId { get; set; }
        public long? CargoCodigo { get; set; }
        public long? CargoDreCodigo { get; set; }
        public long? CargoUeCodigo { get; set; }
        public long? CargoId { get; set; }

        public long? FuncaoCodigo { get; set; }
        public long? FuncaoDreCodigo { get; set; }
        public long? FuncaoUeCodigo { get; set; }
        public long? FuncaoId { get; set; }

        public long? ArquivoId { get; set; }
        public SituacaoInscricao Situacao { get; set; }

        public PropostaTurma PropostaTurma { get; set; }
    }
}
