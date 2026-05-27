namespace SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao
{
    public record InscritoFormacaoDto(
    string CodigoFormacao, string CodigoHomologacao, string NomeFormacao, string AreaPromotora,
    string Dre, string Ue, string Periodo, string SituacaoFormacao, string Modalidade,
    string PublicoAlvo, string Funcao, string Etapa, string Ano, string Componente,
    string Turma, string RfCpf, string Nome, string SituacaoInscricao, string SituacaoConclusao,
    string Email, string EmailNaoEducacional, string Pcd, string QualDeficiencia, string PrecisaAdaptacao, string QualAdaptacao);
}
