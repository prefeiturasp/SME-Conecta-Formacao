using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Extensoes
{
    [ExcludeFromCodeCoverage]
    public static class IntExtensao
    {
        public static string GerarAte(this int valor, int ateValor)
        {
            valor++;
            return (valor > ateValor ? 0 : valor).ToString();
        }
    }
}
