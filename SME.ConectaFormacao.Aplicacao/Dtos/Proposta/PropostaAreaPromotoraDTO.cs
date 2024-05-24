using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaAreaPromotoraDTO
    {
        public string Nome { get; set; }
        public AreaPromotoraTipo Tipo { get; set; }
        public Guid GrupoId { get; set; }
    }
}