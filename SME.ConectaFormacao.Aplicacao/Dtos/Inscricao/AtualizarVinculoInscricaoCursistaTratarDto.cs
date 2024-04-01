namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class AtualizarVinculoInscricaoCursistaTratarDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? CargoCodigo { get; set; }
    }
}