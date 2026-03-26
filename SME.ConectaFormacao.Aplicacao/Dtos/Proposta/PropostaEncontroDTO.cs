using SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaEncontroDto
    {
        public long Id { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
        public TipoEncontro? Tipo { get; set; }
        [MaxLength(200, ErrorMessage = "O local não pode conter mais que 200 caracteres")]
        public string? Local { get; set; }

        public IEnumerable<PropostaEncontroTurmaDto> Turmas { get; set; } = [];
        public IEnumerable<PropostaEncontroDataDto> Datas { get; set; } = [];
    }
}
