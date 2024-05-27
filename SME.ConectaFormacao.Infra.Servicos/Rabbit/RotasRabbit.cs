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

        public const string RealizarImportacaoInscricaoCursistaValidar = "conecta.importacao.inscricao.cursista.validar";
        public const string RealizarImportacaoInscricaoCursistaValidarItem = "conecta.importacao.inscricao.cursista.validar.item";
        public const string ProcessarArquivoDeImportacaoInscricao = "conecta.inscricao.processar.arquivo.importacao";
        public const string ProcessarRegistroDoArquivoDeImportacaoInscricao = "conecta.inscricao.processar.registro.arquivo.importacao";

        public const string AtualizarCargoFuncaoVinculoInscricaoCursista = "conecta.inscricao.cursista.atualizar.cargo.funcao.vinculo";
        public const string AtualizarCargoFuncaoVinculoInscricaoCursistaTratar = "conecta.inscricao.cursista.atualizar.cargo.funcao.vinculo.tratar";

        public const string EnviarEmailDevolverProposta = "conecta.enviar.email.devolver.proposta";
        public const string EncerrarInscricaoAutomaticamente = "conecta.inscricao.encerrar.cursista.inativo";
        public const string EncerrarInscricaoAutomaticamenteTurma = "conecta.inscricao.encerrar.cursista.inativo.turma";
        public const string EncerrarInscricaoAutomaticamenteInscricoes = "conecta.inscricao.encerrar.cursista.inativo.inscricoes";
        public const string EncerrarInscricaoAutomaticamenteUsuarios = "conecta.inscricao.encerrar.cursista.inativo.usuarios";
        
        public const string NotificarPareceristasSobreAtribuicaoPelaDF = "conecta.notificar.pareceristas.sobre.atribuicao.pela.df";
        public const string NotificarDFPeloEnvioParecerPeloParecerista = "conecta.notificar.df.pelo.envio.parecer.pelo.parecerista";
        public const string NotificarAreaPromotoraParaAnaliseParecer = "conecta.notificar.area.promotora.para.analise.parecer";
        public const string NotificarPareceristasParaReanalise = "conecta.notificar.pareceristas.para.reanalise";
        public const string NotificarResponsavelDFSobreReanaliseDoParecerista = "conecta.notificar.responsavel.df.sobre.reanalise.do.parecerista";
        public const string NotificarAreaPromotoraSobreValidacaoFinalPelaDF = "conecta.notificar.area.promotora.sobre.validacao.final.pela.df";
        
        public const string EnviarEmail = "conecta.enviar.email";
        public const string EnviarNotificacao = "conecta.enviar.notificacao";
    }
}