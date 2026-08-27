using System.Diagnostics.CodeAnalysis;
namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    [ExcludeFromCodeCoverage]
    public class AnoLetivoDTO
    {
        public AnoLetivoDTO(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }
}

