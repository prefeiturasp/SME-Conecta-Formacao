using Bogus;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.Mock
{
    public class SincronizarDreMock : BaseMock
    {
        public static IEnumerable<DreNomeAbreviacaoDTO> GerarListaDreEol(int quantidade = 3)
        {
            var faker = new Faker<DreNomeAbreviacaoDTO>();
            faker.RuleFor(x => x.Codigo, f => f.Random.Int(min: 1, max: 100).ToString());
            faker.RuleFor(x => x.Abreviacao, "Dre XYZ");
            faker.RuleFor(x => x.Nome, "Nome Da Dre");
            return faker.Generate(quantidade);
        }
    }
}
