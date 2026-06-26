using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class LogMap : BaseMap<Log>
    {
        public LogMap()
        {
            ToTable("logs");
            Map(c => c.CriadoPor).ToColumn("criado_por");
            Map(c => c.CriadoLogin).ToColumn("criado_login");
            Map(c => c.CriadoEm).ToColumn("criado_em");
            Map(c => c.Entidade).ToColumn("entidade");
            Map(c => c.NivelLog).ToColumn("nivel_log");
            Map(c => c.Mensagem).ToColumn("mensagem");
            Map(c => c.Complemento).ToColumn("complemento");
        }

    }
}
