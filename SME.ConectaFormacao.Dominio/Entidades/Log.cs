using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Log : EntidadeBase
    {
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string Entidade { get; set; } = string.Empty;
        public LogNivel NivelLog { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
    }
}
