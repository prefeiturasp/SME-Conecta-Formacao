using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string? FuncaoCodigo { get; set; }
        public string? FuncaoDreCodigo { get; set; }
        public string? FuncaoUeCodigo { get; set; }
        public long? FuncaoId { get; set; }
        public long? ArquivoId { get; set; }
        public int? TipoVinculo { get; set; }
        public SituacaoInscricao Situacao { get; set; }

        [Column("situacaoanterior")]
        public SituacaoInscricao? SituacaoAnterior { get; set; }
        public OrigemInscricao Origem { get; set; }
        public string MotivoCancelamento { get; set; }

        public PropostaTurma PropostaTurma { get; set; }

        public CargoFuncao Cargo { get; set; }

        public CargoFuncao Funcao { get; set; }
        public Usuario Usuario { get; set; }
    }
}
