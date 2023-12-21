using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class CargoFuncaoDeparaEolMock
    {
        private static Faker<CargoFuncaoDeparaEol> Gerador(long cargoFuncaoId, CargoFuncaoTipo cargoFuncaoTipo)
        {
            var faker = new Faker<CargoFuncaoDeparaEol>();
            faker.RuleFor(r => r.CargoFuncaoId, cargoFuncaoId);
            faker.RuleFor(r => r.CodigoCargoEol, f => cargoFuncaoTipo == CargoFuncaoTipo.Cargo ? f.Random.Long(1, 99999) : null);
            faker.RuleFor(r => r.CodigoFuncaoEol, f => cargoFuncaoTipo == CargoFuncaoTipo.Funcao ? f.Random.Long(1, 99999) : null);

            return faker;
        }

        public static IEnumerable<CargoFuncaoDeparaEol> GerarCargoFuncaoDeparaEol(IEnumerable<CargoFuncao> cargoFuncaos)
        {
            var retorno = new List<CargoFuncaoDeparaEol>();
            foreach (var cargoFuncao in cargoFuncaos)
                retorno.Add(Gerador(cargoFuncao.Id, cargoFuncao.Tipo).Generate());

            return retorno;
        }
    }
}
