using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class RetornoListagemFormacaoDTO
    {
        public long Id { get; set; }
        public string? Titulo { get; set; }
        public string? Periodo { get; set; }
        public string? PeriodoInscricao { get; set; }
        public string? AreaPromotora { get; set; }
        public TipoFormacao TipoFormacao { get; set; }
        public TipoEmissor TipoEmissor { get; set; }
        public string? TipoFormacaoDescricao { get; set; }
        public string? TipoEmissorDescricao { get; set; } = null;
        public Formato Formato { get; set; }
        public string? FormatoDescricao { get; set; }
        public string? ImagemUrl { get; set; }
        public bool InscricaoEncerrada { get; set; }
        public bool CursoComCertificado { get; set; }
        public long? CodigoEventoSigpec { get; set; }
    }
}
