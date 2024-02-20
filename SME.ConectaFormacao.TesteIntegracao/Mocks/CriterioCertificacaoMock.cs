using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class CriterioCertificacaoMock : BaseMock
    {
        private static Faker<Dominio.Entidades.CriterioCertificacao> Gerador(bool excluido = false)
        {
            var faker = new Faker<Dominio.Entidades.CriterioCertificacao>();
            faker.RuleFor(x => x.Descricao, f => f.Commerce.Department());
            faker.RuleFor(x => x.Excluido, excluido);
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<Dominio.Entidades.CriterioCertificacao> GerarCriterioCertificacao(int quantidade, bool excluido)
        {
            return Gerador(excluido).Generate(quantidade);
        }

        public static Dominio.Entidades.CriterioCertificacao GerarCriterioCertificacao(bool excluido = false)
        {
            return Gerador(excluido).Generate();
        }
    }
}