using Bogus;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class DreMock : BaseMock
    {
        public static IEnumerable<Dominio.Entidades.Dre> GerarDreValida(int quantidade = 1)
        {
            var faker = new Faker<Dominio.Entidades.Dre>();
            faker.RuleFor(x => x.Codigo, f => f.Random.Int(min: 1, max: 100).ToString());
            faker.RuleFor(x => x.Abreviacao, "Dre XYZ");
            faker.RuleFor(x => x.Nome, "Nome Da Dre");
            faker.RuleFor(x => x.DataAtualizacao, DateTimeExtension.HorarioBrasilia());
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
    }
}
