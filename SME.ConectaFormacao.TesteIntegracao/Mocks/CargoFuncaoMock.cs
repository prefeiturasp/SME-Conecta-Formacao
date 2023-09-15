using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class CargoFuncaoMock
    {
        private static Faker<Dominio.Entidades.CargoFuncao> Gerador(CargoFuncaoTipo? cargoFuncaoTipo = null, bool outros = false)
        {
            var opcoesCargoFuncao = new CargoFuncaoTipo[] { CargoFuncaoTipo.Cargo, CargoFuncaoTipo.Funcao };

            if (outros)
                opcoesCargoFuncao = new CargoFuncaoTipo[] { CargoFuncaoTipo.Outros };

            var faker = new Faker<Dominio.Entidades.CargoFuncao>();
            faker.RuleFor(dest => dest.Nome, f => f.Commerce.Department());
            faker.RuleFor(dest => dest.Tipo, f => cargoFuncaoTipo.HasValue ? cargoFuncaoTipo : f.PickRandom(opcoesCargoFuncao));
            faker.RuleFor(dest => dest.Outros, outros);
            faker.RuleFor(dest => dest.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(dest => dest.CriadoPor, f => f.Person.FullName);
            faker.RuleFor(dest => dest.CriadoLogin, f => f.Person.FirstName);
            return faker;
        }

        public static IEnumerable<Dominio.Entidades.CargoFuncao> GerarCargoFuncao(int quantidade, bool outros = false)
        {
            return Gerador(outros: outros).Generate(quantidade);
        }

        public static Dominio.Entidades.CargoFuncao GerarCargoFuncao(CargoFuncaoTipo cargoFuncaoTipo, bool outros)
        {
            return Gerador(cargoFuncaoTipo, outros).Generate();
        }
    }
}
