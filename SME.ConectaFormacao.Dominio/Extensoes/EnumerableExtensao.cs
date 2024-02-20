namespace SME.ConectaFormacao.Dominio.Extensoes
{
    public static class EnumerableExtensao
    {
        public static bool NaoPossuiElementos<T>(this IEnumerable<T> items)
        {
            return items.EhNulo() || !items.Any();
        }
        
        public static bool PossuiElementos<T>(this IEnumerable<T> items)
        {
            return items.NaoEhNulo() && items.Any();
        }
    }
}
