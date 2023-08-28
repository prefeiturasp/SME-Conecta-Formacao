using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.CargoFuncao.Mocks
{
    public class CargoFuncaoMock
    {
        public static IEnumerable<Dominio.Entidades.CargoFuncao> GerarCargoFuncao(int quantidade)
        {
            var faker = new Faker<Dominio.Entidades.CargoFuncao>();
            faker.RuleFor(dest => dest.Nome, f => f.Commerce.Department());
            faker.RuleFor(dest => dest.Tipo, f => f.PickRandom<CargoFuncaoTipo>());
            faker.RuleFor(dest => dest.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(dest => dest.CriadoPor, f => f.Person.FullName);
            faker.RuleFor(dest => dest.CriadoLogin, f => f.Person.FirstName);
            return faker.Generate(quantidade);
        }
    }
}
