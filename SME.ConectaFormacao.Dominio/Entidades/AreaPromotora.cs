using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class AreaPromotora : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public AreaPromotoraTipo Tipo { get; set; }
        public string Email { get; set; }
        public Guid GrupoId { get; set; }
        public long? DreId { get; set; }
        public Dre? Dre { get; set; }
        public IEnumerable<AreaPromotoraTelefone> Telefones { get; set; }

        public void AdicionarDre(Dre? dre)
        {
            Dre = dre;
            DreId = dre?.Id;
        }
    }
}