namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class FiltroListagemInscricaoDTO
    {
        public string? RegistroFuncional { get; set; }
        public string? Cpf { get; set; }
        public string? NomeCursista { get; set; }
        public long[]? TurmasId { get; set; }
        public bool OcultarCancelada { get; set; } = false;
        public bool OcultarTransferida { get; set; } = false;
    }
}
