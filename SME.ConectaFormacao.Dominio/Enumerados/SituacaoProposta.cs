﻿using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoProposta
    {
        [Display(Name = "Publicada", Prompt = "#297805")]
        Publicada = 1,

        [Display(Name = "Rascunho", Prompt = "#EEC25E")]
        Rascunho = 2,

        [Display(Name = "Cadastrada", Prompt = "#6464FF")]
        Cadastrada = 3,

        [Display(Name = "Aguardando análise da DF", Prompt = "#000000")]
        AguardandoAnaliseDf = 4,

        [Display(Name = "Aguardando análise da gestão", Prompt = "#000000")]
        AguardandoAnaliseGestao = 5,

        [Display(Name = "Desfavorável", Prompt = "#D06D12")]
        Desfavoravel = 6,

        [Display(Name = "Devolvida", Prompt = "#D06D12")]
        Devolvida = 7,

        [Display(Name = "Alterando", Prompt = "#297805")]
        Alterando = 8,

        [Display(Name = "Aprovada", Prompt = "#008000")]
        Aprovada = 9,

        [Display(Name = "Aguardando análise do Parecerista", Prompt = "#000000")]
        AguardandoAnalisePeloParecerista = 10,

        [Display(Name = "Aguardando análise do parecer pela DF", Prompt = "#000000")]
        AguardandoAnaliseParecerPelaDF = 11,

        [Display(Name = "Análise do parecer pela área promotora", Prompt = "#000000")]
        AnaliseParecerPelaAreaPromotora = 12,

        [Display(Name = "Recusada", Prompt = "#008000")]
        Recusada = 13,

        [Display(Name = "Aguardando reanálise do Parecerista", Prompt = "#000000")]
        AguardandoReanalisePeloParecerista = 14,

        [Display(Name = "Aguardando validação final pela DF", Prompt = "#000000")]
        AguardandoValidacaoFinalPelaDF = 15
    }

    public static class SituacaoPropostaExtensao
    {
        public static bool EstaPublicada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Publicada;
        }

        public static bool EstaCadastrada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Cadastrada;
        }

        public static bool EstaAguardandoAnaliseDf(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnaliseDf;
        }

        public static bool EstaDevolvida(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Devolvida;
        }

        public static bool EhParaSalvarRascunho(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Rascunho || valor == SituacaoProposta.Alterando;
        }

        public static bool EhAlterando(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Alterando;
        }

        public static bool EhAprovada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Aprovada;
        }

        public static bool EstaAguardandoAnaliseParecerPelaDFOuAreaPromotoraOuAnaliseFinalPelaDF(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnaliseParecerPelaDF
                   || valor == SituacaoProposta.AnaliseParecerPelaAreaPromotora
                   || valor == SituacaoProposta.AguardandoValidacaoFinalPelaDF;
        }

        public static bool EstaAguardandoAnalisePeloParecerista(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnalisePeloParecerista;
        }

        public static bool EstaAguardandoAnaliseParecerPelaDF(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnaliseParecerPelaDF;
        }

        public static bool EstaAguardandoAnaliseParecerFinalPelaDF(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoValidacaoFinalPelaDF;
        }

        public static bool EstaAnaliseParecerPelaAreaPromotora(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AnaliseParecerPelaAreaPromotora;
        }

        public static bool EstaAguardandoReanalisePeloParecerista(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoReanalisePeloParecerista;
        }

        public static bool EstaAprovadaOuRecusada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Aprovada || valor == SituacaoProposta.Recusada;
        }

        public static bool EstaAprovada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Aprovada;
        }

        public static bool EstaRecusada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Recusada;
        }

        public static bool EstaAguardandoAnalisePeloPareceristaOuAguardandoAnaliseParecerPelaDF(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnalisePeloParecerista || valor == SituacaoProposta.AguardandoAnaliseParecerPelaDF;
        }
    }
}
