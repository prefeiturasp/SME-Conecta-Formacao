using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros
{
    public class CronogramaEncontroDto
    {
        public long Id { get; set; }
        public List<string> NomeTurmas { get; set; } = [];
        public int QuantidadeDiasEncontro { get; set; }
        public TipoEncontro? Tipo { get; set; }
        public List<CronogramaDataEncontroDto> CronogramaDatas { get; set; } = [];
    }
}
