using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class MinhasDeclaracoesCodafDto
    {
        public long Id { get; set; }
        public long CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public long CodigoDeclaracao { get; set; }
        public bool TemRf { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; set; }
        public DateTime DataEmissao { get; set; }
    }
}
