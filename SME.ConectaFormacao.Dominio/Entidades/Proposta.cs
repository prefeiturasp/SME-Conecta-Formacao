using Dommel;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Proposta : EntidadeBaseAuditavel
    {
        public long AreaPromotoraId { get; set; }
        public FormacaoHomologada? FormacaoHomologada { get; set; }
        public TipoFormacao? TipoFormacao { get; set; }
        public Formato? Formato { get; set; }
        public string NomeFormacao { get; set; }
        public short? QuantidadeTurmas { get; set; }
        public short? QuantidadeVagasTurma { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public string FuncaoEspecificaOutros { get; set; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
        public long? ArquivoImagemDivulgacaoId { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoInicio { get; set; }
        public DateTime? DataInscricaoFim { get; set; }
        public string CargaHorariaPresencial { get; set; }
        public string CargaHorariaSincrona { get; set; }
        public string CargaHorariaDistancia { get; set; }
        public string Justificativa { get; set; }
        public string Objetivos { get; set; }
        public string ConteudoProgramatico { get; set; }
        public string ProcedimentoMetadologico { get; set; }
        public string Referencia { get; set; }
        public bool CursoComCertificado { get; set; }
        public bool AcaoInformativa { get; set; }
        public string? DescricaoDaAtividade { get; set; }
        public string? AcaoFormativaTexto { get; set; }
        public string? AcaoFormativaLink { get; set; }
        public bool IntegrarNoSGA { get; set; }

        public AreaPromotora AreaPromotora { get; set; }
        public Arquivo ArquivoImagemDivulgacao { get; set; }
        public IEnumerable<PropostaDre> Dres { get; set; }
        public IEnumerable<PropostaPublicoAlvo> PublicosAlvo { get; set; }
        public IEnumerable<PropostaFuncaoEspecifica> FuncoesEspecificas { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricao> CriteriosValidacaoInscricao { get; set; }
        public IEnumerable<PropostaVagaRemanecente> VagasRemanecentes { get; set; }
        public IEnumerable<PropostaEncontro> Encontros { get; set; }
        public IEnumerable<PropostaPalavraChave> PalavrasChaves { get; set; }
        public IEnumerable<PropostaCriterioCertificacao> CriterioCertificacao { get; set; }
        public IEnumerable<PropostaRegente> Regentes { get; set; }
        public IEnumerable<PropostaTutor> Tutores { get; set; }
        public IEnumerable<PropostaTurma> Turmas { get; set; }
        public IEnumerable<PropostaTurmaDre> TurmasDres { get; set; }
        public IEnumerable<PropostaModalidade> Modalidades { get; set; }
        public IEnumerable<PropostaAnoTurma> AnosTurmas { get; set; }
        public IEnumerable<PropostaComponenteCurricular> ComponentesCurriculares { get; set; }
        public PropostaMovimentacao Movimentacao { get; set; }
        public IEnumerable<PropostaTurmaDre> ObterPropostaTurmasDres
        {
            get
            {
                if (Turmas.EhNulo())
                    return default;

                return from propostaTurma in Turmas
                       from dreId in propostaTurma.DresIds
                       select new PropostaTurmaDre()
                       {
                           PropostaTurmaId = propostaTurma.Id,
                           DreId = dreId
                       };
            }
        }
        public IEnumerable<PropostaTipoInscricao> TiposInscricao { get; set; }
        
        public bool EstaEmPeriodoDeInscricao
        {
            get
            {
                if (!DataInscricaoInicio.HasValue && !DataInscricaoFim.HasValue)
                    return false;

                return DataInscricaoInicio.Value.Date <= DateTimeExtension.HorarioBrasilia().Date &&
                       DataInscricaoFim.Value.Date >= DateTimeExtension.HorarioBrasilia().Date;
            }
        }

    }
}
