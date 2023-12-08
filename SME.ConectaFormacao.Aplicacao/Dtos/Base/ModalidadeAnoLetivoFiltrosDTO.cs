
using System.ComponentModel.DataAnnotations;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Base
{
    public class ModalidadeAnoLetivoFiltrosDTO
    {
        [Required(ErrorMessage = "A modalidade deve ser informada.")]
        public Modalidade Modalidade { get; set; }
        
        [Required(ErrorMessage = "O ano letivo deve ser informado.")]
        public int AnoLetivo { get; set; }
    }
}
