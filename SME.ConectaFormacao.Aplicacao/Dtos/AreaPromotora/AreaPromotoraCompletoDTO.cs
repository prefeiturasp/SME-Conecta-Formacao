using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora
{
    public class AreaPromotoraCompletoDTO
    {
        public string Nome { get; set; }
        public AreaPromotoraTipo Tipo { get; set; }
        public string TipoDescricao => Tipo.Nome();
        public string Email { get; set; }
        public Guid GrupoId { get; set; }
        public IEnumerable<AreaPromotoraTelefoneDTO> Telefones { get; set; }
        public AuditoriaDTO Auditoria { get; set; }
    }
}
