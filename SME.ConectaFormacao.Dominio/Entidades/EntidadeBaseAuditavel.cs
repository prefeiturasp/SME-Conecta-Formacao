namespace SME.ConectaFormacao.Dominio;

public abstract class EntidadeBaseAuditavel : EntidadeBase
{
    public DateTime? AlteradoEm { get; set; }
    public string? AlteradoPor { get; set; }
    public string? AlteradoLogin { get; set; }
    public DateTime CriadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? CriadoLogin { get; set; }

    public void ManterCriador(EntidadeBaseAuditavel entidade)
    {
        CriadoEm = entidade.CriadoEm;
        CriadoPor = entidade.CriadoPor;
        CriadoLogin = entidade.CriadoLogin;
    }
}
