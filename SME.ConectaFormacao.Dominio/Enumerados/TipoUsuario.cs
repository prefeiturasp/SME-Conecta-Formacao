namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoUsuario
    {
        Interno = 1,
        Externo = 2
    }

    public static class TipoUsuarioExtensao
    {
        public static bool EhInterno(this TipoUsuario valor)
        {
            return valor == TipoUsuario.Interno;
        }

        public static bool EhExterno(this TipoUsuario valor)
        {
            return valor == TipoUsuario.Externo;
        }
    }
}