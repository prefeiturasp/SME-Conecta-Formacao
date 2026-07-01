namespace SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias
{
    public class CoordenadoriaDto
    {
        public long Id { get; set; }
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public string? NomeComSigla { get; set; }
    }
}