namespace SME.ConectaFormacao.Infra.Servicos.Eol;

public class DreServicoEol
{
    public DreServicoEol()
    {
    }

    public DreServicoEol(string codigo, string nome, string abreviacao, long id = 0)
    {
        Codigo = codigo;
        Nome = nome;
        Abreviacao = abreviacao;
        Id = id;
    }

    public long Id { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public string Abreviacao { get; set; }
}