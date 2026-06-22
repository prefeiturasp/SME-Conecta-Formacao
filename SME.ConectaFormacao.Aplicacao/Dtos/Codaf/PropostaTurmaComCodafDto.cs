namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class PropostaTurmaComCodafDto
    {
        public long Id { get; set; }
        public required string Descricao { get; set; }
        public long CodafId { get; set; }
    }
}
