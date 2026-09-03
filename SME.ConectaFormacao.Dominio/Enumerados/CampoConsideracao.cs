using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum CampoConsideracao
    {
        [Display(Name = "FormaÃ§Ã£o homologada por SME/COPED/DF")]
        FormacaoHomologada = 1,

        [Display(Name = "Tipo de formaÃ§Ã£o")]
        TipoFormacao = 2,

        [Display(Name = "Modalidade formativa")]
        Formato = 3,

        [Display(Name = "Tipo de inscriÃ§Ã£o")]
        TiposInscricao = 4,

        [Display(Name = "Integrar no SGA")]
        IntegrarNoSGA = 5,

        [Display(Name = "DRE")]
        Dres = 6,

        [Display(Name = "Nome da formaÃ§Ã£o")]
        NomeFormacao = 7,

        [Display(Name = "PÃºblico alvo")]
        PublicosAlvo = 8,

        [Display(Name = "FunÃ§Ã£o especÃ­fica")]
        FuncoesEspecificas = 9,

        [Display(Name = "Etapa modalidade")]
        Modalidade = 10,

        [Display(Name = "Ano/Etapa")]
        AnosTurmas = 11,

        [Display(Name = "Componente Curricular")]
        ComponentesCurriculares = 12,

        [Display(Name = "CritÃ©rios para validaÃ§Ã£o das inscriÃ§Ãµes")]
        CriteriosValidacaoInscricao = 13,

        [Display(Name = "CritÃ©rios para validaÃ§Ã£o das inscriÃ§Ãµes")]
        VagasRemanecentes = 14,

        [Display(Name = "Quantidade de turmas")]
        QuantidadeTurmas = 15,

        [Display(Name = "Vagas por turma")]
        QuantidadeVagasTurma = 16,

        [Display(Name = "Carga horÃ¡ria")]
        CargaHoraria = 17,

        [Display(Name = "Justificativa")]
        Justificativa = 18,

        [Display(Name = "Objetivos")]
        Objetivos = 19,

        [Display(Name = "ConteÃºdo ProgramÃ¡tico")]
        ConteudoProgramatico = 20,

        [Display(Name = "Procedimentos metodolÃ³gicos")]
        ProcedimentoMetadologico = 21,

        [Display(Name = "ReferÃªncias")]
        Referencia = 22,

        [Display(Name = "Palavras-chave")]
        PalavrasChaves = 23,

        [Display(Name = "PerÃ­odo de realizaÃ§Ã£o")]
        PeriodoRealizacao = 24,

        [Display(Name = "PerÃ­odo de inscriÃ§Ã£o")]
        PeriodoInscricao = 25,

        [Display(Name = "Curso com certificaÃ§Ã£o")]
        CursoComCertificado = 26,

        [Display(Name = "CritÃ©rios para certificaÃ§Ã£o")]
        CriterioCertificacao = 27,

        [Display(Name = "DescriÃ§Ã£o da atividade obrigatÃ³ria para certificaÃ§Ã£o")]
        DescricaoDaAtividade = 28,

        [Display(Name = "DescriÃ§Ã£o do cÃ³digo dos evento (SIGPEC)")]
        DescricaoCodigoEventoSigpec = 29,

        [Display(Name = "DescriÃ§Ã£o do link para as nscriÃ§Ãµes")]
        DescricaoLinkParaInscricoesExterna = 30,

        [Display(Name = "DescriÃ§Ã£o das funÃ§Ãµes especÃ­ficas (outros)',")]
        DescricaoFuncoesEspecificasOutros = 31,

        [Display(Name = "DescriÃ§Ã£o dos critÃ©rios para validaÃ§Ã£o das inscriÃ§Ãµes (outros)")]
        DescricaoCriteriosValidacaoInscricaoOutros = 32
    }

    [ExcludeFromCodeCoverage]
    public static class CampoParecerExtensao
    {
        public static bool EhCampoFormacaoHomologada(this CampoConsideracao valor)
        {
            return valor == CampoConsideracao.FormacaoHomologada;
        }
    }
}

