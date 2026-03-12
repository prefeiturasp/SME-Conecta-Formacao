namespace SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao
{
    public record RelatorioInscritosFormacaoDto(
    string NomeUsuario,
    string Rf,
    DateTime DataGeracao,
    IEnumerable<InscritoFormacaoDto> Inscritos);
}
