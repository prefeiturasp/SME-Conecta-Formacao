using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;

namespace SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular
{
    public class ComponenteCurricularEAnoTurmaFiltrosDTO : ModalidadeAnoLetivoFiltrosDTO
    {
        [Required(ErrorMessage = "O ano deve ser informada.")]
        public int AnoTurmaId { get; set; }
    }
}
