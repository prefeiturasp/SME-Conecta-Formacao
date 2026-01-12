namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas
{
    public class CodafComentarioDevolucaoDto
    {
        public long Id { get; set; }
        public long CodafListaPresencaId { get; set; }
        public string Comentario { get; set; } = null!;
        public string CriadoPor { get; set; } = null!;
        public string CriadoLogin { get; set; } = null!;
        public DateTime CriadoEm { get; set; }
    }
}