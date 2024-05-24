using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class ImportacaoArquivoRegistroMock : BaseMock
    {
        public static IEnumerable<ImportacaoArquivoRegistro> GerarImportacaoArquivo(
                                                long ImportacaoArquivoId,
                                                SituacaoImportacaoArquivoRegistro situacao,
                                                int quantidade = 1)
        {
            var faker = new Faker<ImportacaoArquivoRegistro>();
            faker.RuleFor(x => x.ImportacaoArquivoId, ImportacaoArquivoId);
            faker.RuleFor(x => x.Linha, f => f.IndexVariable);
            faker.RuleFor(x => x.Conteudo, f => f.Company.CompanyName());
            faker.RuleFor(x => x.Situacao, situacao);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }

        public static IEnumerable<ImportacaoArquivoRegistro> GerarImportacaoArquivo(
                                        long ImportacaoArquivoId,
                                        string conteudo,
                                        string erro,
                                        int linha,
                                        SituacaoImportacaoArquivoRegistro situacao,
                                        int quantidade = 1)
        {
            var faker = new Faker<ImportacaoArquivoRegistro>();
            faker.RuleFor(x => x.ImportacaoArquivoId, ImportacaoArquivoId);
            faker.RuleFor(x => x.Linha, linha);
            faker.RuleFor(x => x.Conteudo, conteudo);
            faker.RuleFor(x => x.Erro, erro);
            faker.RuleFor(x => x.Situacao, situacao);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
        
        public static IEnumerable<ImportacaoArquivoRegistro> GerarImportacaoArquivoCarregamentoInicial(
            long ImportacaoArquivoId,
            int quantidade = 1)
        {
            var faker = new Faker<ImportacaoArquivoRegistro>();
            faker.RuleFor(x => x.ImportacaoArquivoId, ImportacaoArquivoId);
            faker.RuleFor(x => x.Linha, f => f.IndexVariable);
            faker.RuleFor(x => x.Conteudo, f => f.Company.CompanyName());
            faker.RuleFor(x => x.Situacao, SituacaoImportacaoArquivoRegistro.CarregamentoInicial);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
    }
}
