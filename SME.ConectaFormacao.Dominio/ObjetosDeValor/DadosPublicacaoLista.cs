namespace SME.ConectaFormacao.Dominio.ObjetosDeValor
{
    public record DadosPublicacaoLista(
        DateTime? DataPublicacao,
        DateTime? DataPublicacaoDom,
        short? NumeroComunicado,
        short? PaginaComunicadoDom,
        int? CodigoCursoEol,
        int? CodigoNivel,
        string? Observacao
    );
}
