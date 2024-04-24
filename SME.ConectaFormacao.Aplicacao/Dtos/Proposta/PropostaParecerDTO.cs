using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaParecerDTO
    {
        public long? Id { get; set; }
        public long PropostaId { get; set; }
        public CampoParecer Campo { get; set; }
        public string Descricao { get; set; }
    }
}
