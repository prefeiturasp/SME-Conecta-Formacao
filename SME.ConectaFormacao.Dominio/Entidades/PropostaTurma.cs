namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTurma : EntidadeBaseAuditavel, ICloneable
    {
        public long PropostaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public long[] DresIds { get; set; } = [];

        public IEnumerable<PropostaTurmaDre> Dres { get; set; } = [];
        public Proposta Proposta { get; set; } = new();

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
