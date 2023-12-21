using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CargoFuncaoDeparaEolMap : BaseMap<CargoFuncaoDeparaEol>
    {
        public CargoFuncaoDeparaEolMap()
        {
            ToTable("cargo_funcao_depara_eol");
            Map(c => c.CargoFuncaoId).ToColumn("cargo_funcao_id");
            Map(c => c.CodigoCargoEol).ToColumn("codigo_cargo_eol");
            Map(c => c.CodigoFuncaoEol).ToColumn("codigo_funcao_eol");
        }
    }
}
