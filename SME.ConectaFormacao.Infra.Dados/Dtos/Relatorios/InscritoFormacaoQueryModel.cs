using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios
{
    public class InscritoFormacaoQueryModel
    {
        public string CodigoFormacao { get; set; } = string.Empty;
        public string CodigoHomologacao { get; set; } = string.Empty;
        public string NomeFormacao { get; set; } = string.Empty;
        public string AreaPromotora { get; set; } = string.Empty;
        public string? Dre { get; set; }
        public string? Ue { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public SituacaoProposta? SituacaoFormacao { get; set; }
        public Formato? ModalidadeFormativa { get; set; }
        public string? PublicoAlvo { get; set; }
        public string? FuncaoEspecifica { get; set; }
        public Modalidade? EtapaModalidade { get; set; }
        public string? AnoEtapa { get; set; }
        public string? ComponenteCurricular { get; set; }
        public string? Turma { get; set; }
        public string? RfCpf { get; set; }
        public string? NomeCursista { get; set; }
        public SituacaoInscricao? SituacaoInscricao { get; set; }
        public string? SituacaoConclusaoCursista { get; set; }
        public string? Email { get; set; }
        public bool? Pcd { get; set; }
        public string? DescricaoDeficiencia { get; set; }
        public bool NecessitaAdaptacao { get; set; }
        public string? DescricaoAdaptacao { get; set; }
    }
}
