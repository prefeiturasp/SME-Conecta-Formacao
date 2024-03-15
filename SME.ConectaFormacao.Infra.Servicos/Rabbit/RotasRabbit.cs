namespace SME.ConectaFormacao.Infra
{
    public static class RotasRabbit
    {
        public const string SincronizaEstruturaInstitucionalDre = "conecta.sincronizacao.institucional.dre";
        public const string SincronizaEstruturaInstitucionalDreTratar = "conecta.sincronizacao.institucional.dre.tratar";

        public const string SincronizaComponentesCurricularesEAnosTurmaEOL = "conecta.sincronizacao.componentes.curriculares.ano.turma.eol";

        public const string GerarPropostaTurmaVaga = "conecta.proposta.turma.vaga";
        public const string RealizarInscricaoAutomatica = "conecta.inscricao.automatica";
        public const string RealizarInscricaoAutomaticaTratarTurmas = "conecta.inscricao.automatica.tratar.turmas";
        public const string RealizarInscricaoAutomaticaTratarCursistas = "conecta.inscricao.automatica.tratar.cursistas";
        public const string RealizarInscricaoAutomaticaIncreverCursista = "conecta.inscricao.automatica.inscrever.cursista";
        public const string ProcessarArquivoDeImportacaoInscricao = "conecta.inscricao.processar.arquivo.importacao";
    }
}