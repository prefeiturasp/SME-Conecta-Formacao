namespace SME.ConectaFormacao.Infra
{
    public class InscricaoPossuiAnexoDTO
    {
        public long InscricaoId { get; set; }
        public string? NomeArquivo { get; set; }
        public Guid? Codigo { get; set; }
    }
}