namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class DreNomeAbreviacaoDTO
{
    public DreNomeAbreviacaoDTO()
    {
    }

    public DreNomeAbreviacaoDTO(string codigo, string nome, string abreviacao, long id = 0)
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