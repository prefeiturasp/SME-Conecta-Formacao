using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks
{
    public class ArquivoMock
    {
        private static Faker<Dominio.Entidades.Arquivo> Gerador(TipoArquivo? tipo)
        {
            var faker = new Faker<Dominio.Entidades.Arquivo>("pt_BR");

            faker.RuleFor(t => t.Nome, f => f.Name.FirstName() + ".jpg");
            faker.RuleFor(t => t.Codigo, Guid.NewGuid());
            faker.RuleFor(t => t.TipoConteudo, "image/jpg");
            faker.RuleFor(t => t.Tipo, f => tipo.HasValue ? tipo : f.PickRandom<TipoArquivo>());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            faker.RuleFor(x => x.AlteradoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.AlteradoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AlteradoLogin, f => f.Name.FirstName());

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
