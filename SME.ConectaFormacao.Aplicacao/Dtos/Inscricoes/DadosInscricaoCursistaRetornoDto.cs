namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class DadosInscricaoCursistaRetornoDto
    {
        public long Id { get; set; }
        public long InscricaoId { get; set; }
        public string Documento { get; set; } = null!;
        public string Nome { get; set; } = null!;
    }
}
