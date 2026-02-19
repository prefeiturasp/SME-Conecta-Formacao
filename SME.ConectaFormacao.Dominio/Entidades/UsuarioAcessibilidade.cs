using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public sealed class UsuarioAcessibilidade : EntidadeBaseAuditavel, IEquatable<UsuarioAcessibilidade>
    {
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public bool? PossuiDeficiencia { get; set; }
        public string? DescricaoDeficiencia { get; set; }
        public bool? NecessitaAdaptacao { get; set; }
        public string? DescricaoAdaptacao { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UsuarioAcessibilidade);
        }
        public bool Equals(UsuarioAcessibilidade? other)
        {
            if(other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return UsuarioId == other.UsuarioId &&
                   PossuiDeficiencia == other.PossuiDeficiencia &&
                   DescricaoDeficiencia.SaoStringsIguais(other.DescricaoDeficiencia) &&
                   NecessitaAdaptacao == other.NecessitaAdaptacao &&
                   DescricaoAdaptacao.SaoStringsIguais(other.DescricaoAdaptacao) &&
                   Excluido == other.Excluido;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UsuarioId, PossuiDeficiencia, 
                DescricaoDeficiencia?.Trim().ToLowerInvariant(), NecessitaAdaptacao, 
                DescricaoAdaptacao?.Trim().ToLowerInvariant(), Excluido);
        }

        public static bool operator ==(UsuarioAcessibilidade? left, UsuarioAcessibilidade? right)
        {
            if (left is null) return right is null;
            return Equals(left, right);
        }

        public static bool operator !=(UsuarioAcessibilidade? left, UsuarioAcessibilidade? right)
        {
            return !Equals(left, right);
        }
    }
}