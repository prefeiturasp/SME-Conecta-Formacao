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
        public int? QuantidadeTurmas { get; set; }
        public int? QuantidadeVagasTurma { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public string FuncaoEspecificaOutros { get; set; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
        public long? ArquivoImagemDivulgacaoId { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }

        public AreaPromotora AreaPromotora { get; set; }
        public IEnumerable<PropostaPublicoAlvo> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecifica> FuncoesEspecificas { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricao> CriteriosValidacaoInscricao { get; set; }
        public IEnumerable<PropostaVagaRemanecente> VagasRemanecentes { get; set; }
    }
}
