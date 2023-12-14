namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaTurmaCompletoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public PropostaTurmaDreCompletoDTO[] Dres { get; set; }
    }
}
