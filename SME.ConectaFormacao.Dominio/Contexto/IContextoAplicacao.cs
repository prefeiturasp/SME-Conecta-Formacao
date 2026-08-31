using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Contexto;

public interface IContextoAplicacao
{
    IDictionary<string, object> Variaveis { get; set; }

    string UsuarioLogado { get; }
    string LoginUsuario { get; }
    string NomeUsuario { get; }
    string PerfilUsuario { get; }
    Permissao[] Permissoes { get; }
    Guid? IdPerfilUsuario => !string.IsNullOrWhiteSpace(PerfilUsuario) ? Guid.Parse(PerfilUsuario) : null;
    bool EhAdministrador => IdPerfilUsuario == Perfis.ADMIN_DF || IdPerfilUsuario == Perfis.EMFORPEF;
    string Administrador { get; }
    T? ObterVariavel<T>(string nome);

    IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);
    void AdicionarVariaveis(IDictionary<string, object> variaveis);
}