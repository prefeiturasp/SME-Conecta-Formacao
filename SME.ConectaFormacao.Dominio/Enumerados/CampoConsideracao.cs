using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum CampoConsideracao
    {
        [Display(Name = "Formação homologada por SME/COPED/DF")]
        FormacaoHomologada = 1,

        [Display(Name = "Tipo de formação")]
        TipoFormacao = 2,

        [Display(Name = "Modalidade formativa")]
        Formato = 3,

        [Display(Name = "Tipo de inscrição")]
        TiposInscricao = 4,

        [Display(Name = "Integrar no SGA")]
        IntegrarNoSGA = 5,

        [Display(Name = "DRE")]
        Dres = 6,

        [Display(Name = "Nome da formação")]
        NomeFormacao = 7,

        [Display(Name = "Público alvo")]
        PublicosAlvo = 8,

        [Display(Name = "Função específica")]
        FuncoesEspecificas = 9,

        [Display(Name = "Etapa modalidade")]
        Modalidade = 10,

        [Display(Name = "Ano/Etapa")]
        AnosTurmas = 11,

        [Display(Name = "Componente Curricular")]
        ComponentesCurriculares = 12,

        [Display(Name = "Critérios para validação das inscrições")]
        CriteriosValidacaoInscricao = 13,

        [Display(Name = "Critérios para validação das inscrições")]
        VagasRemanecentes = 14,

        [Display(Name = "Quantidade de turmas")]
        QuantidadeTurmas = 15,

        [Display(Name = "Vagas por turma")]
        QuantidadeVagasTurma = 16,

        [Display(Name = "Carga horária")]
        CargaHoraria = 17,

        [Display(Name = "Justificativa")]
        Justificativa = 18,

        [Display(Name = "Objetivos")]
        Objetivos = 19,

        [Display(Name = "Conteúdo Programático")]
        ConteudoProgramatico = 20,

        [Display(Name = "Procedimentos metodológicos")]
        ProcedimentoMetadologico = 21,

        [Display(Name = "Referências")]
        Referencia = 22,

        [Display(Name = "Palavras-chave")]
        PalavrasChaves = 23,

        [Display(Name = "Período de realização")]
        PeriodoRealizacao = 24,

        [Display(Name = "Período de inscrição")]
        PeriodoInscricao = 25,

        [Display(Name = "Curso com certificação")]
        CursoComCertificado = 26,

        [Display(Name = "Critérios para certificação")]
        CriterioCertificacao = 27,

        [Display(Name = "Descrição da atividade obrigatória para certificação")]
        DescricaoDaAtividade = 28,

        [Display(Name = "Descrição do código dos evento (SIGPEC)")]
        DescricaoCodigoEventoSigpec = 29,

        [Display(Name = "Descrição do link para as nscrições")]
        DescricaoLinkParaInscricoesExterna = 30,

        [Display(Name = "Descrição das funções específicas (outros)',")]
        DescricaoFuncoesEspecificasOutros = 31,

        [Display(Name = "Descrição dos critérios para validação das inscrições (outros)")]
        DescricaoCriteriosValidacaoInscricaoOutros = 32
    }

    public static class CampoParecerExtensao
    {
        public static bool EhCampoFormacaoHomologada(this CampoConsideracao valor)
        {
            return valor == CampoConsideracao.FormacaoHomologada;
        }
    }
}