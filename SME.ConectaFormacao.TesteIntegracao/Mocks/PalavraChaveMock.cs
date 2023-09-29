using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PalavraChaveMock : BaseMock
    {
        private static Faker<Dominio.Entidades.PalavraChave> Gerador()
        {
            var faker = new Faker<Dominio.Entidades.PalavraChave>();
            faker.RuleFor(dest => dest.Nome, f => f.Commerce.Department());
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<Dominio.Entidades.PalavraChave> GerarPalavrasChaves(int quantidade)
        {
            return Gerador().Generate(quantidade);
        }

        public static Dominio.Entidades.PalavraChave GerarPalavraChave()
        {
            return Gerador().Generate();
        }
    }
}
