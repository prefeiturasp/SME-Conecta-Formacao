using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CargoFuncaoMap : BaseMapAuditavel<CargoFuncao>
    {
        public CargoFuncaoMap()
        {
            ToTable("cargo_funcao");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Outros).ToColumn("outros");
            Map(c => c.Ordem).ToColumn("ordem");
        }
    }
}
