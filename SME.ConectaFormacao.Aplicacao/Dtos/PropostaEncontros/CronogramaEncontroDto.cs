using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros
{
    public class CronogramaEncontroDto
    {
        public long Id { get; set; }
        public List<PropostaEncontroTurmaDto> Turmas { get; set; } = [];
        public int QuantidadeDiasEncontro { get; set; }
        public TipoEncontro? Tipo { get; set; }
        public string? Local { get; set; }
        public List<CronogramaDataEncontroDto> CronogramaDatas { get; set; } = [];
    }
}
