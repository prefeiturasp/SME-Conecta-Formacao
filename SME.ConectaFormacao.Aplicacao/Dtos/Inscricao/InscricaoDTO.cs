using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoDTO
    {
        public long PropostaTurmaId { get; set; }
        [Required(ErrorMessage ="E-mail é obrigatório")]
        public string Email { get; set; }
        public long? CargoId { get; set; }
        public long? FuncaoId { get; set; }
        public long? ArquivoId { get; set; }
    }
}
