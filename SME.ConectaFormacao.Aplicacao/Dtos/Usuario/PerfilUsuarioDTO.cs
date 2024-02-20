namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
public struct PerfilUsuarioDTO
{
    public PerfilUsuarioDTO(Guid perfil, string perfilNome)
    {
        Perfil = perfil;
        PerfilNome = perfilNome;
    }
    public Guid Perfil { get; set; }
    public string PerfilNome { get; set; }
}
