namespace SME.ConectaFormacao.Aplicacao.Dtos.Grupo
{
    public class GrupoDTO : IEquatable<GrupoDTO>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int VisaoId { get; set; }
        
        
        public bool Equals(GrupoDTO? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Nome == other.Nome;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GrupoDTO)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nome);
        }
    }
}
