namespace SME.ConectaFormacao.Aplicacao.Dtos
{
    public class RetornoListagemFormacaoDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Periodo { get; set; }
        public string AreaPromotora { get; set; }
        public string TipoFormacao { get; set; }
        public string Formato { get; set; }
        public string ImagemUrl { get; set; }
        public bool InscricaoEncerrada { get; set; }
    }
}
