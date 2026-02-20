using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Comparadores
{
    public class UsuarioAcessibilidadeComparadorDados : IEqualityComparer<UsuarioAcessibilidade>
    {
        public static UsuarioAcessibilidadeComparadorDados Instancia { get; } = new();

        public bool Equals(UsuarioAcessibilidade? x, UsuarioAcessibilidade? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.UsuarioId == y.UsuarioId &&
                   x.PossuiDeficiencia == y.PossuiDeficiencia &&
                   x.DescricaoDeficiencia.SaoStringsIguais(y.DescricaoDeficiencia) &&
                   x.NecessitaAdaptacao == y.NecessitaAdaptacao &&
                   x.DescricaoAdaptacao.SaoStringsIguais(y.DescricaoAdaptacao);
        }

        public int GetHashCode([DisallowNull] UsuarioAcessibilidade obj)
        {
            return HashCode.Combine(
                obj.UsuarioId,
                obj.PossuiDeficiencia,
                obj.DescricaoDeficiencia?.Trim().ToLowerInvariant(),
                obj.NecessitaAdaptacao,
                obj.DescricaoAdaptacao?.Trim().ToLowerInvariant()
            );
        }
    }
}
