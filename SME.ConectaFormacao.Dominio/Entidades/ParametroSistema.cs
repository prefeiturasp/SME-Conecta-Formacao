using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ParametroSistema : EntidadeBaseAuditavel
    {
        public int? Ano { get; set; }
        public bool Ativo { get; set; }
        public string Descricao { get; set; }
        public string Nome { get; set; }
        public TipoParametroSistema Tipo { get; set; }
        public string Valor { get; set; }
    }
}