﻿using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaCompletoDTO
    {
        public bool EhAdminDF;
        public FormacaoHomologada? FormacaoHomologada { get; set; }
        public TipoFormacao? TipoFormacao { get; set; }
        public Formato? Formato { get; set; }
        public TipoInscricao? TipoInscricao { get; set; }
        public string? NomeFormacao { get; set; }
        public short? QuantidadeTurmas { get; set; }
        public short? QuantidadeVagasTurma { get; set; }
        public string CargaHorariaPresencial { get; set; }
        public string CargaHorariaSincrona { get; set; }
        public string CargaHorariaDistancia { get; set; }
        public string Justificativa { get; set; }
        public string Objetivos { get; set; }
        public string ConteudoProgramatico { get; set; }
        public string ProcedimentoMetadologico { get; set; }
        public string Referencia { get; set; }
        public string PalavrasChavesOutros { get; set; }
        public int QuantidadeTotal => QuantidadeTurmas.GetValueOrDefault() * QuantidadeVagasTurma.GetValueOrDefault();
        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecificaDTO> FuncoesEspecificas { get; set; }
        public string FuncaoEspecificaOutros { get; set; }
        public IEnumerable<PropostaVagaRemanecenteDTO> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricaoDTO> CriteriosValidacaoInscricao { get; set; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
        public string? PublicoAlvoOutros { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public string NomeSituacao { get; set; }
        public PropostaImagemDivulgacaoDTO? ArquivoImagemDivulgacao { get; set; }
        public AuditoriaDTO Auditoria { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }
        public IEnumerable<PropostaPalavraChaveDTO> PalavrasChaves { get; set; }
        public IEnumerable<CriterioCertificacaoDTO>? CriterioCertificacao { get; set; }
        public bool CursoComCertificado { get; set; }
        public bool AcaoInformativa { get; set; }
        public string? DescricaoDaAtividade { get; set; }
        public string? AcaoFormativaTexto { get; set; }
        public string? AcaoFormativaLink { get; set; }
        public IEnumerable<PropostaTurmaCompletoDTO> Turmas { get; set; }
        public IEnumerable<PropostaModalidadeDTO> Modalidades { get; set; }
        public IEnumerable<PropostaAnoTurmaDTO> AnosTurmas { get; set; }
        public IEnumerable<PropostaComponenteCurricularDTO> ComponentesCurriculares { get; set; }
        public IEnumerable<PropostaDreDTO> Dres { get; set; }
        public IEnumerable<PropostaTipoInscricaoDTO> TiposInscricao { get; set; }
        public bool? IntegrarNoSGA { get; set; }
        public bool DesativarAnoEhComponente { get; set; }
        public string? RfResponsavelDf { get; set; }
        public PropostaMovimentacaoDTO Movimentacao { get; set; }
        public PropostaAreaPromotoraDTO AreaPromotora { get; set; }
        public string? UltimaJustificativaDevolucao { get; set; }
        public string? LinkParaInscricoesExterna { get; set; }
        public long? CodigoEventoSigpec { get; set; }
        public long? NumeroHomologacao { get; set; }
        public IEnumerable<PropostaTotalConsideracaoDTO> TotalDeConsideracoes { get; set; }
        public bool ExibirConsideracoes { get; set; }
        public bool PodeEnviar { get; set; }
        public bool PodeEnviarConsideracoes { get; set; }
        public int QtdeLimitePareceristaProposta { get; set; }
        public IEnumerable<PropostaPareceristaDTO> Pareceristas { get; set; }
        public string LabelRecusar { get; set; }
        public string LabelAprovar { get; set; }
        public bool PodeAprovar { get; set; }
        public bool PodeRecusar { get; set; }
        public bool EhParecerista { get; set; }
        public bool EhAreaPromotora { get; set; }
        public string? UltimaJustificativaAprovacaoRecusa { get; set; }
        public string? CargaHorariaTotal { get; set; }
        public string? CargaHorariaNaoPresencial { get; set; }
        public string? OutrosCriterios { get; set; }
        public int? HorasTotais { get; set; }
        public string? CargaHorariaTotalOutra { get; set; }
    }
}
