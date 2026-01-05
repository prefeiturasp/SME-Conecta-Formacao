using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes
{
    public class FiltroListagemInscricaoDto
    {
        public required long PropostaId { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? Cpf { get; set; }
        public string? NomeCursista { get; set; }
        public long[]? TurmasId { get; set; }
        public bool OcultarCancelada { get; set; } = false;
        public bool OcultarTransferida { get; set; } = false;
        public SituacaoInscricao? Situacao { get; set; }
        public int? CargoFuncaoId { get; set; }
        public required int NumeroPagina { get; set; }
        public required int NumeroRegistros { get; set; }
    }
}
