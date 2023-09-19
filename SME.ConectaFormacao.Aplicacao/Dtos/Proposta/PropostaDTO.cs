using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaDTO
    {
        public TipoFormacao? TipoFormacao { get; set; }
        public Modalidade? Modalidade { get; set; }
        public TipoInscricao? TipoInscricao { get; set; }
        public string? NomeFormacao { get; set; }
        public int? QuantidadeTurmas { get; set; }
        public int? QuantidadeVagasTurma { get; set; }
        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecificaDTO> FuncoesEspecificas { get; set; }
        public string FuncaoEspecificaOutros { get; set; }
        public IEnumerable<PropostaVagaRemanecenteDTO> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricaoDTO> CriteriosValidacaoInscricao { get; set; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public long? ArquivoImagemDivulgacaoId { get; set; }
    }
}
