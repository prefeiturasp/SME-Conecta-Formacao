using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class GrupoGestaoMock : BaseMock
    {
        public static IEnumerable<Dominio.Entidades.GrupoGestao> GerarGrupoGestaoValida(int quantidade = 1)
        {
            var faker = new Faker<Dominio.Entidades.GrupoGestao>();
            faker.RuleFor(x => x.GrupoId, f => Guid.NewGuid());
            faker.RuleFor(x => x.Nome, f => f.Lorem.Sentence());
            return faker.Generate(quantidade);
        }
    }
}
