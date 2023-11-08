namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class DreNomeAbreviacaoDTO
{
    public DreNomeAbreviacaoDTO()
    {
    }

    public DreNomeAbreviacaoDTO(string codigo, string nome, string abreviacao)
    {
        Codigo = codigo;
        Nome = nome;
        Abreviacao = abreviacao;
    }

    public string Codigo { get; set; }
    public string Nome { get; set; }
    public string Abreviacao { get; set; }
}