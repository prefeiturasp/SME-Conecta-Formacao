using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class DadosPrincipaisRelatorioCodafDto
    {
        public long CodafId { get; set; }
        public long TurmaId { get; set; }
        public string NomeTurma { get; set; } = string.Empty;
        public int QuantidadeVagasTurma { get; set; }
        public string NomeAreaPromotora { get; set; } = string.Empty;
        public TipoFormacao TipoFormacao { get; set; }
        public string NomeFormacao { get; set; } = string.Empty;
        public int QuantidadeTurmas { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public bool CursoComCertificado { get; set; }
        public int NumeroHomologacao { get; set; }
        public int CodigoEventoSigpec { get; set; }
        public int CargaHorariaTotal { get; set; }
        public string? CargaHorariaDistancia { get; set; }
        public string? CargaHorariaPresencial { get; set; }
        public string? CargaHorariaSincrona { get; set; }
        public Formato TipoFormato { get; set; }
        public short NumeroComunicado { get; set; }
        public DateTime DataPublicacao { get; set; }
        public DateTime DataPublicacaoDom { get; set; }
        public short PaginaComunicadoDom { get; set; }
        public string? NomeDre { get; set; }
        public string? Observacao { get; set; }
        public string? ObservacaoCodafSuplementar { get; set; }
        public DateTime DataCodaf { get; set; }

        public IEnumerable<DataAulaTurmaRelatorioCodafDto> DataAulas { get; set; } = [];
        public IEnumerable<DadosRegenteTurmaRelatorioCodafDto> RegentesTurma { get; set; } = [];
        public IEnumerable<DadosParticipanteRelatorioCodafDto>? Participantes { get; set; }
        public IEnumerable<DadosRetificacaoRelatorioCodafDto>? Retificacoes { get; set; }
    }
}

