using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoManualDTO
    {
        public long PropostaTurmaId { get; set; }
        public bool ProfissionalRede { get; set; }
        public bool PodeContinuar { get; set; } = false;
        public string RegistroFuncional { get; set; }
        [Required(ErrorMessage = "CPF é obrigatório")]
        public string Cpf { get; set; }
        public int? TipoVinculo { get; set; }
    }
}
