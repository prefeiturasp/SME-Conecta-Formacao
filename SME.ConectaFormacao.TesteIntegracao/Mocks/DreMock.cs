using Bogus;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class DreMock : BaseMock
    {
        public static IEnumerable<Dominio.Entidades.Dre> GerarDreValida(int quantidade = 1, bool todos = true)
        {
            var faker = new Faker<Dominio.Entidades.Dre>();
            faker.RuleFor(x => x.Codigo, f => f.Random.Int(min: 1, max: quantidade).ToString());
            faker.RuleFor(x => x.Abreviacao, f => f.Company.CompanySuffix());
            faker.RuleFor(x => x.Nome, f => f.Company.CompanyName());
            faker.RuleFor(x => x.DataAtualizacao, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.Todos, todos);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
    }
}
