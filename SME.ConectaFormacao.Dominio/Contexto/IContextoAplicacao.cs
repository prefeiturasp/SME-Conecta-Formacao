namespace SME.ConectaFormacao.Dominio.Contexto;

public interface IContextoAplicacao
{
    IDictionary<string, object> Variaveis { get; set; }

    string UsuarioLogado { get; }
    string NomeUsuario { get; }
    string PerfilUsuario { get; }
    IEnumerable<string> Perfis { get; }
    IEnumerable<string> Dres { get; }
    string Administrador { get; }
    T ObterVariavel<T>(string nome);

    IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);
    void AdicionarVariaveis(IDictionary<string, object> variaveis);
}