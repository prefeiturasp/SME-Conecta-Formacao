using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Contexto;

[ExcludeFromCodeCoverage]
public abstract class ContextoBase : IContextoAplicacao
{
    protected ContextoBase()
    {
        Variaveis = new Dictionary<string, object>();
    }

    public string NomeUsuario => ObterVariavel<string>("NomeUsuario") ?? "Sistema";
    public string UsuarioLogado => ObterVariavel<string>("UsuarioLogado") ?? "Sistema";
    public string PerfilUsuario => ObterVariavel<string>("PerfilUsuario") ?? string.Empty;
    public IDictionary<string, object> Variaveis { get; set; }
    public string Administrador => ObterVariavel<string>("Administrador") ?? string.Empty;
    public Permissao[] Permissoes => ObterVariavel<Permissao[]>("Permissoes") ?? [];

    public string LoginUsuario => ObterVariavel<string>("login") ?? "Sistema";
    public virtual Guid? IdPerfilUsuario => !string.IsNullOrWhiteSpace(PerfilUsuario) && Guid.TryParse(PerfilUsuario, out var id) ? id : null;
    public virtual bool EhAdministrador => IdPerfilUsuario == Perfis.ADMIN_DF || IdPerfilUsuario == Perfis.EMFORPEF;
    public virtual bool EhNeerDc => IdPerfilUsuario == Perfis.NEER_DC;

    public abstract void AdicionarVariaveis(IDictionary<string, object> variaveis);
    public abstract IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);

    public T? ObterVariavel<T>(string nome)
    {

        if (Variaveis.TryGetValue(nome, out object valor))
            return (T)valor;

        return default;
    }
}

