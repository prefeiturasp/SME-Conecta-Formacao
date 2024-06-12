using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaPareceristaConsideracaoDTO
    {
        public long Id { get; set; }

        public CampoConsideracao Campo { get; set; }

        public string Descricao { get; set; }

        public bool PodeAlterar { get; set; }

        public AuditoriaDTO? Auditoria { get; set; }
    }
}
