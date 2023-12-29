using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Inscricao : EntidadeBaseAuditavel
    {
        public long PropostaTurmaId { get; set; }
        public long UsuarioId { get; set; }
        public string CargoCodigo { get; set; }
        public string CargoDreCodigo { get; set; }
        public string CargoUeCodigo { get; set; }
        public long? CargoId { get; set; }

        public string FuncaoCodigo { get; set; }
        public string FuncaoDreCodigo { get; set; }
        public string FuncaoUeCodigo { get; set; }
        public long? FuncaoId { get; set; }

        public long? ArquivoId { get; set; }
        public SituacaoInscricao Situacao { get; set; }

        public PropostaTurma PropostaTurma { get; set; }
    }
}
