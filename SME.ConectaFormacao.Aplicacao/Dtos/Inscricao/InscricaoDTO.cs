using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoDTO
    {
        public long PropostaTurmaId { get; set; }
        [Required(ErrorMessage ="E-mail é obrigatório")]
        public string Email { get; set; }
        public long? ArquivoId { get; set; }
        
        public long? CargoId { get; set; }//Remover
        public long? CargoCodigo { get; set; }
        public long? CargoDreCodigo { get; set; }
        public long? CargoUeCodigo { get; set; }

        public long? FuncaoCodigo { get; set; }
        public long? FuncaoDreCodigo { get; set; }
        public long? FuncaoUeCodigo { get; set; }
    }
}
