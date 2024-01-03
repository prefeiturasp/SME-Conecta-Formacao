using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class PropostaInformacoesCadastranteDTO
    {
        public string UsuarioLogadoNome { get; set; }
        public string UsuarioLogadoEmail { get; set; }
        public string AreaPromotora { get; set; }
        public string AreaPromotoraTipo { get; set; }
        public AreaPromotoraTipo AreaPromotoraTipoId { get; set; }
        public string AreaPromotoraTelefones { get; set; }
        public string AreaPromotoraEmails { get; set; }
    }
}
