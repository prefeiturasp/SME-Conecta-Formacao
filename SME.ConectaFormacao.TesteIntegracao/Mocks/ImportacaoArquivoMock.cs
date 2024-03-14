using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class ImportacaoArquivoMock : BaseMock
    {
        public static IEnumerable<ImportacaoArquivo> GerarImportacaoArquivo(
                                                        long propostaId,
                                                        SituacaoImportacaoArquivo situacao,
                                                        int quantidade = 1)
        {
            var faker = new Faker<ImportacaoArquivo>();
            faker.RuleFor(x => x.Nome, f => f.Company.CompanyName());
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.Situacao, situacao);
            faker.RuleFor(x => x.Tipo, TipoImportacaoArquivo.Inscricao_Manual);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
    }
}
