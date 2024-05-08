using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaPareceristaConsideracaoMock : BaseMock
    {
        private static Faker<PropostaPareceristaConsideracao> Gerador()
        {
            var faker = new Faker<PropostaPareceristaConsideracao>("pt_BR");
            faker.RuleFor(x => x.Campo, f => (CampoParecer)f.Random.Short(1,28));
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker;
        }

        public static PropostaPareceristaConsideracao GerarPropostaPareceristaConsideracao()
        {
            return Gerador().Generate();
        }
        
        public static PropostaPareceristaConsideracao GerarPropostaPareceristaConsideracao(long propostaPareceristaId, CampoParecer campoParecer)
        {
            var propostaParecer = GerarPropostaPareceristaConsideracao();
            propostaParecer.PropostaPareceristaId = propostaPareceristaId;
            propostaParecer.Campo = campoParecer;
            return propostaParecer;
        }
        
        public static IEnumerable<PropostaPareceristaConsideracao> GerarPropostasPareceristasConsideracoes(int quantidade = 10)
        {
            return Gerador().Generate(quantidade);
        }
    }
}
