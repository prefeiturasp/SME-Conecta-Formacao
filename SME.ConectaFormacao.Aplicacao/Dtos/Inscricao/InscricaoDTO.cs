using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoDTO
    {
        public long PropostaTurmaId { get; set; }
        public long? ArquivoId { get; set; }
        public string? CargoCodigo { get; set; }
        public string? CargoDreCodigo { get; set; }
        public string? CargoUeCodigo { get; set; }

        public string? FuncaoCodigo { get; set; }
        public string? FuncaoDreCodigo { get; set; }
        public string? FuncaoUeCodigo { get; set; }

        public int? TipoVinculo { get; set; }
    }
}
