﻿using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class AreaPromotora : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public AreaPromotoraTipo? Tipo { get; set; }
        public string Email { get; set; }
        public Guid? PerfilId { get; set; }
        public IEnumerable<AreaPromotoraTelefone> Telefones { get; set; }
    }
}
