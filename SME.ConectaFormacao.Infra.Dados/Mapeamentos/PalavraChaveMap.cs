using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PalavraChaveMap : BaseMapAuditavel<PalavraChave>
    {
        public PalavraChaveMap()
        {
            ToTable("palavra_chave");
            Map(c => c.Nome).ToColumn("nome");
        }
    }
}
