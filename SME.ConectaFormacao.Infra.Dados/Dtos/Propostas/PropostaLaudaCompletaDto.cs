namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class PropostaLaudaCompletaDto
    {
        public long Id { get; set; }
        public string NomeAreaPromotora { get; set; } = string.Empty;
        public string TipoAreaPromotora { get; set; } = string.Empty;
        public string TipoFormacaoConecta { get; set; } = string.Empty;
        public string Modalidade { get; set; } = string.Empty;
        public string Justificativa { get; set; } = string.Empty;
        public string Objetivos { get; set; } = string.Empty;
        public string ConteudoProgramatico { get; set; } = string.Empty;
        public string Procedimentos { get; set; } = string.Empty;
        public string Referencias { get; set; } = string.Empty;
        public string DescricaoAtividade { get; set; } = string.Empty;
        public string NomeFormacao { get; set; } = string.Empty;
        public string CargaHorariaPresencial { get; set; } = string.Empty;
        public string CargaHorariaSincrona { get; set; } = string.Empty;
        public string CargaHorariaDistancia { get; set; } = string.Empty;
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public int QuantidadeTurmas { get; set; }
        public int QuantidadeVagasTurmas { get; set; }
        public string NumeroHomologacao { get; set; } = string.Empty;


        public DateTime DataInscricaoInicio { get; set; }
        public DateTime DataInscricaoFim { get; set; }
        public string LinkInscricaoExterna { get; set; } = string.Empty;

        public string PublicoAlvo_Outros { get; set; } = string.Empty;
        public string FuncaoEspecifica_Outros { get; set; } = string.Empty;
        public string Criterios_Outros { get; set; } = string.Empty;
        public string CriteriosValidacao_Outros { get; set; } = string.Empty;
        public int CodigoEventoSigpec { get; set; }

        public IEnumerable<PropostaPublicoAlvoDto> PublicosAlvo { get; set; } = [];
        public IEnumerable<PropostaPublicoAlvoDto> FuncaoEspecifica { get; set; } = [];
        public IEnumerable<PropostaPublicoAlvoDto> VagasRemanecentes { get; set; } = [];
        public IEnumerable<PropostaPublicoAlvoDto> CriteriosValidacao { get; set; } = [];
        public IEnumerable<PropostaPublicoAlvoDto> CriteriosCertificacao { get; set; } = [];
        public IEnumerable<RegenteLaudaDto> Regentes { get; set; } = [];
        public IEnumerable<string> TelefonesAreaPromotora { get; set; } = [];

        public IEnumerable<TurmaLaudaDto> CronogramaTurmas { get; set; } = [];
    }

    public class PropostaPublicoAlvoDto
    {
        public string Nome { get; set; } = string.Empty;
    }

    public partial class RegenteLaudaDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Rf { get; set; } = string.Empty;
        public string MiniBio { get; set; } = string.Empty;
        public bool ProfissionalDaRede { get; set; }

        [System.Text.RegularExpressions.GeneratedRegex("<.*?>")]
        private static partial System.Text.RegularExpressions.Regex HtmlTagsRegex();

        public string ObterDescricaoCompleta()
        {
            var descricao = new System.Text.StringBuilder(Nome.ToUpper());

            if (!string.IsNullOrWhiteSpace(Rf))
            {
                descricao.Append($" - RF: {Rf}");
            }

            if (!string.IsNullOrWhiteSpace(MiniBio))
            {
                var miniBioTexto = HtmlTagsRegex().Replace(MiniBio, string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(miniBioTexto) && miniBioTexto != "-")
                {
                    descricao.Append($" - {miniBioTexto}");
                }
            }

            return descricao.ToString();
        }
    }

    public class TurmaLaudaDto
    {
        public string Identificacao { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFim { get; set; } = string.Empty;

        public string DatasFormatadas =>
            (DataInicio.HasValue && DataFim.HasValue)
                ? $"{DataInicio.Value:dd/MM/yyyy} A {DataFim.Value:dd/MM/yyyy}"
                : "";

        public string HorariosFormatados =>
            (!string.IsNullOrEmpty(HoraInicio) && !string.IsNullOrEmpty(HoraFim))
                ? $"DAS {FormatarHora(HoraInicio)} ÀS {FormatarHora(HoraFim)}"
                : "";

        private static string FormatarHora(string hora)
        {
            if (TimeSpan.TryParse(hora, System.Globalization.CultureInfo.InvariantCulture, out var time))
            {
                return time.Minutes == 0
                    ? $"{time.Hours}H"
                    : $"{time.Hours}H{time.Minutes:D2}";
            }
            return hora;
        }
    }
}
