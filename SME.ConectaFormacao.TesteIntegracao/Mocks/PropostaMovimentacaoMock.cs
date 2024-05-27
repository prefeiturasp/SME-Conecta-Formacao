using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaMovimentacaoMock : BaseMock
    {
        private static Faker<PropostaMovimentacao> Gerador(long propostaId, SituacaoProposta situacaoProposta = SituacaoProposta.Aprovada)
        {
            var faker = new Faker<PropostaMovimentacao>("pt_BR");
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(dest => dest.Justificativa, f => f.Lorem.Sentence(100));
            faker.RuleFor(dest => dest.Situacao, situacaoProposta);
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker;
        }

        public static PropostaMovimentacao GerarPropostaMovimentacao(long propostaId, SituacaoProposta situacaoProposta = SituacaoProposta.Aprovada)
        {
            return Gerador(propostaId, situacaoProposta).Generate();
        }

        public static IEnumerable<PropostaMovimentacao> GerarPropostasMovimentacao(long propostaId, SituacaoProposta situacaoProposta = SituacaoProposta.Aprovada, int quantidade = 10)
        {
            return Gerador(propostaId, situacaoProposta).Generate(quantidade);
        }
    }
}
