using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaDTO
    {
        public TipoFormacao? TipoFormacao { get; set; }
        public Modalidade? Modalidade { get; set; }
        public TipoInscricao? TipoInscricao { get; set; }
        [MaxLength(150, ErrorMessage = "O nome da formação não pode conter mais que 150 caracteres")]
        public string? NomeFormacao { get; set; }
        public short? QuantidadeTurmas { get; set; }
        public short? QuantidadeVagasTurma { get; set; }
        public string? FuncaoEspecificaOutros { get; set; }
        public string? CriterioValidacaoInscricaoOutros { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public long? ArquivoImagemDivulgacaoId { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }

        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecificaDTO> FuncoesEspecificas { get; set; }
        public IEnumerable<PropostaVagaRemanecenteDTO> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricaoDTO> CriteriosValidacaoInscricao { get; set; }
    }
}
