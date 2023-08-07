namespace SME.ConectaFormacao.Infra.Servicos.Acessos
{
    public struct AcessosPerfilUsuario
    {
        public AcessosPerfilUsuario(Guid perfil, string perfilNome)
        {
            Perfil = perfil;
            PerfilNome = perfilNome;
        }
        public Guid Perfil { get; set; }
        public string PerfilNome { get; set; }
    }
}
