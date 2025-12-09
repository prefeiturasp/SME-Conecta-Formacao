namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
    {
        public long Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? CargoCodigo { get; set; }
    }
}