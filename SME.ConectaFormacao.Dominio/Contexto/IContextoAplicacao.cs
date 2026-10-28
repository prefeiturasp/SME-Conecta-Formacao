using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Contexto;

public interface IContextoAplicacao
{
    IDictionary<string, object> Variaveis { get; set; }

    string UsuarioLogado { get; }
    string LoginUsuario { get; }
    string NomeUsuario { get; }
    string PerfilUsuario { get; }
    Permissao[] Permissoes { get; }
    Guid? IdPerfilUsuario { get; }
    bool EhAdministrador { get; }
    bool EhNeerDc { get; }
    string Administrador { get; }
    T? ObterVariavel<T>(string nome);

    IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);
    void AdicionarVariaveis(IDictionary<string, object> variaveis);
}