using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios
{
    public class FiltroRelatorioInscritosPorFormacaoDto
    {
        public long? PropostaId { get; set; }
        public long? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long? PropostaTurmaId { get; set; }
        public Formato? Formato { get; set; }
        public long? AreaPromotoraId { get; set; }
        private DateTime _periodoDeRealizacaoInicial;
        public DateTime PeriodoDeRealizacaoInicial
        {
            get => _periodoDeRealizacaoInicial;
            set => _periodoDeRealizacaoInicial = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        }

        private DateTime _periodoDeRealizacaoFinal;
        public DateTime PeriodoDeRealizacaoFinal
        {
            get => _periodoDeRealizacaoFinal;
            set => _periodoDeRealizacaoFinal = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        }
        public SituacaoProposta? SituacaoProposta { get; set; }
        public SituacaoInscricao? SituacaoInscricao { get; set; }
        public long? CargoPublicoAlvoId { get; set; }
        public long? FuncaoId { get; set; }
        public Modalidade? Modalidade { get; set; }
        public long? AnoTurmaId { get; set; }
        public long? ComponenteCurricularId { get; set; }
        public long? DreId { get; set; }
        public Guid? UeId { get; set; }
        public string? DocumentoCursista { get; set; }
        public string? Email { get; set; }
        public bool? Pcd { get; set; }
        public bool? NecessitaAdaptacao { get; set; }
    }
}
