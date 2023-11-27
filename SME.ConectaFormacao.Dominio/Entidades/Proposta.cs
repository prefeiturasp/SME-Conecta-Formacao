using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Proposta : EntidadeBaseAuditavel
    {
        public long AreaPromotoraId { get; set; }
        public TipoFormacao? TipoFormacao { get; set; }
        public Modalidade? Modalidade { get; set; }
        public TipoInscricao? TipoInscricao { get; set; }
        public string NomeFormacao { get; set; }
        public short? QuantidadeTurmas { get; set; }
        public short? QuantidadeVagasTurma { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public string FuncaoEspecificaOutros { get; set; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
        public long? ArquivoImagemDivulgacaoId { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }

        public AreaPromotora AreaPromotora { get; set; }
        public string CargaHorariaPresencial { get; set; }
        public string CargaHorariaSincrona { get; set; }
        public string CargaHorariaDistancia { get; set; }
        public string Justificativa { get; set; }
        public string Objetivos { get; set; }
        public string ConteudoProgramatico { get; set; }
        public string ProcedimentoMetadologico { get; set; }
        public string Referencia { get; set; }
        public IEnumerable<PropostaPublicoAlvo> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecifica> FuncoesEspecificas { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricao> CriteriosValidacaoInscricao { get; set; }
        public IEnumerable<PropostaVagaRemanecente> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaEncontro> Encontros { get; set; }
        public IEnumerable<PropostaPalavraChave> PalavrasChaves { get; set; }
        public IEnumerable<PropostaCriterioCertificacao> CriterioCertificacao { get; set; }
        public IEnumerable<PropostaRegente> Regentes { get; set; } = new List<PropostaRegente>();
        public IEnumerable<PropostaTutor> Tutores { get; set; } = new List<PropostaTutor>();
        public bool CursoComCertificado { get; set; }
        public bool AcaoInformativa { get; set; }
        public string? DescricaoDaAtividade { get; set; }
        public string? AcaoFormativaTexto { get; set; }
        public string? AcaoFormativaLink { get; set; }
        public bool? FormacaoHomologada { get; set; }
        public long? GrupoGestaoId { get; set; }
    }
}
