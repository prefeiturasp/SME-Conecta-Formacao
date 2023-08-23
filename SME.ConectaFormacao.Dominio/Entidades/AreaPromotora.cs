using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class AreaPromotora : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public AreaPromotoraTipo Tipo { get; set; }
        public string Email { get; set; }
        public Guid GrupoId { get; set; }
        public IEnumerable<AreaPromotoraTelefone> Telefones { get; set; }

        public void Alterar(string nome, AreaPromotoraTipo tipo, string email, Guid grupoId)
        {
            Nome = nome;
            Tipo = tipo;
            Email = email;
            GrupoId = grupoId;
        }
    }
}
