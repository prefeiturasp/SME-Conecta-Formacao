namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class AutocompletarNumeroHomologacaoDto 
    {
        public long PropostaId { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoFormacao { get; set; }
    }
}