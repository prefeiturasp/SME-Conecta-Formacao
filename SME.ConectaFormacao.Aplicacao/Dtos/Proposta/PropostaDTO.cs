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
        public string? CargaHorariaPresencial { get; set; }
        public string? CargaHorariaSincrona { get; set; }
        public string? CargaHorariaDistancia { get; set; }
        public string? Justificativa { get; set; }
        public string? Objetivos { get; set; }
        public string? ConteudoProgramatico { get; set; }
        public string? ProcedimentoMetadologico { get; set; }
        public string? Referencia { get; set; }
        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecificaDTO> FuncoesEspecificas { get; set; }
        public IEnumerable<PropostaVagaRemanecenteDTO> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricaoDTO> CriteriosValidacaoInscricao { get; set; }
        public IEnumerable<PropostaEncontroDTO>? Encontros { get; set; }
        public IEnumerable<PropostaPalavraChaveDTO> PalavrasChaves { get; set; }
        public IEnumerable<CriterioCertificacaoDTO> CriterioCertificacao { get; set; }
        public bool CursoComCertificado { get; set; }
        public bool AcaoInformativa { get; set; }
        public string? DescricaoDaAtividade { get; set; }
        public string? AcaoFormativaTexto { get; set; }
        public string? AcaoFormativaLink { get; set; }
        public bool FormacaoHomologada { get; set; }
    }
}
