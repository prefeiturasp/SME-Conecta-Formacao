using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class ArquivoMock : BaseMock
    {
        private static Faker<Dominio.Entidades.Arquivo> Gerador(TipoArquivo? tipo)
        {
            var faker = new Faker<Dominio.Entidades.Arquivo>("pt_BR");

            faker.RuleFor(t => t.Nome, f => f.Name.FirstName() + ".jpg");
            faker.RuleFor(t => t.Codigo, Guid.NewGuid());
            faker.RuleFor(t => t.TipoConteudo, "image/jpg");
            faker.RuleFor(t => t.Tipo, f => tipo.HasValue ? tipo : f.PickRandom<TipoArquivo>());
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);

            return faker;
        }

        public static IEnumerable<Dominio.Entidades.Arquivo> GerarArquivo(int quantidade, TipoArquivo? tipo = null)
        {
            return Gerador(tipo).Generate(quantidade);
        }

        public static Dominio.Entidades.Arquivo GerarArquivo(TipoArquivo? tipo = null)
        {
            return Gerador(tipo).Generate();
        }
    }
}
