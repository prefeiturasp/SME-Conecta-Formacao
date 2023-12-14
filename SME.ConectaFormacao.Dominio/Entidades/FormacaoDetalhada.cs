using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class FormacaoDetalhada
    {
        public TipoFormacao? TipoFormacao { get; set; }
        public Formato? Formato { get; set; }
        public string NomeFormacao { get; set; }
        public DateTime? DataRealizacaoInicio { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
        public DateTime? DataInscricaoFim { get; set; }
        public string Justificativa { get; set; }

        public string AreaPromotora { get; set; }
        public IEnumerable<string> PublicosAlvo { get; set; }
        public IEnumerable<string> PalavrasChaves { get; set; }
        public IEnumerable<FormacaoTurma> Turmas { get; set; }
        public string ImagemUrl { get; set; }
    }
}
