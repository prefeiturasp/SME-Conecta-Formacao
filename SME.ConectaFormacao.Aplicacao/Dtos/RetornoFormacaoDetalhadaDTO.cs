using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class RetornoFormacaoDetalhadaDTO
    {
        public string Titulo { get; set; }
        public string AreaPromotora { get; set; }
        public TipoFormacao TipoFormacao { get; set; }
        public string TipoFormacaoDescricao { get; set; }
        public Formato Formato { get; set; }
        public string FormatoDescricao { get; set; }
        public string Periodo { get; set; }
        public string Justificativa { get; set; }
        public string[] PublicosAlvo { get; set; }
        public string[] PalavrasChaves { get; set; }
        public bool InscricaoEncerrada { get; set; }
        public string ImagemUrl { get; set; }
        public FormacaoHomologada FormacaoHomologada { get; set; }
        public DateTime DataInscricaoFim { get; set; }

        public IEnumerable<RetornoTurmaDetalheDTO> Turmas { get; set; }
    }
}
