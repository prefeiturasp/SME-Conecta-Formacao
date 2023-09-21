using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public abstract class BaseMocks
    {
        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Long(1);
        }
    }
}
