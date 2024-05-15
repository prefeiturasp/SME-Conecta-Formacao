namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoUsuario
    {
        Interno = 1,
        Externo = 2,
        RedeParceria = 3
    }

    public static class TipoUsuarioExtensao
    {
        public static bool EhInterno(this TipoUsuario tipo)
        {
            return tipo == TipoUsuario.Interno;
        }

        public static bool EhExterno(this TipoUsuario tipo)
        {
            return tipo == TipoUsuario.Externo;
        }

        public static bool EhRedeParceria(this TipoUsuario tipo)
        {
            return tipo == TipoUsuario.RedeParceria;
        }
    }
}