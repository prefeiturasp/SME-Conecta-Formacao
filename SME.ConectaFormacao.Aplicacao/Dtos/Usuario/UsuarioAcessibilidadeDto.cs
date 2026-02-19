namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public record UsuarioAcessibilidadeDto
        (long Id, long UsuarioId, bool? PossuiDeficiencia, string? DescricaoDeficiencia,
        bool? NecessitaAdaptacao, string? DescricaoAdaptacao, bool Salvar);
}