using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaEncontroDTO
    {
        public long Id { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
        public TipoEncontro? Tipo { get; set; }
        [MaxLength(200, ErrorMessage = "O local não pode conter mais que 200 caracteres")]
        public string? Local { get; set; }

        public IEnumerable<PropostaEncontroTurmaDTO> Turmas { get; set; }
        public IEnumerable<PropostaEncontroDataDTO> Datas { get; set; }
    }
}
