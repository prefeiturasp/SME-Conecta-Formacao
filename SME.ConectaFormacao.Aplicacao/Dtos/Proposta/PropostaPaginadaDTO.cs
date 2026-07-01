using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPaginadaDTO
    {
        public long Id { get; set; }
        public string TipoFormacao { get; set; } = null!;
        public TipoEmissor? TipoEmissor { get; set; } = null!;
        public long? IdEmissor { get; set; } = null!;
        public string AreaPromotora { get; set; } = null!;
        public string Formato { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
        public long NumeroHomologacao { get; set; }
        public string DataRealizacaoInicio { get; set; } = null!;
        public string DataRealizacaoFim { get; set; } = null!;
        public string Situacao { get; set; } = null!;
        public FormacaoHomologada FormacaoHomologada { get; set; }
        public string Revalidacao { get; set; } = null!;
    }
}
