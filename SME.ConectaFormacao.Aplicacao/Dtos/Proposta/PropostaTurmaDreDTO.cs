using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaTurmaDreDTO
    {
        public long Id { get; set; }
        public long PropostaTurmaId { get; set; }
        public PropostaTurma PropostaTurma { get; set; }
        public long? DreId { get; set; }
        public string? DreNome { get; set; }
    }
}
