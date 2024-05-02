using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaPareceristaMock : BaseMock
    {
        private static Faker<PropostaParecerista> Gerador(long propostaId, string registroFuncional, string nomeParecerista)
        {
            var faker = new Faker<PropostaParecerista>("pt_BR");
            faker.RuleFor(x => x.PropostaId, f => propostaId);
            faker.RuleFor(x => x.RegistroFuncional, registroFuncional);
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.NomeParecerista, nomeParecerista);
            AuditoriaFaker(faker);
            return faker;
        }

        public static PropostaParecerista GerarPropostaParecerista(long propostaId, string registroFuncional, string nomeParecerista)
        {
            return Gerador(propostaId, registroFuncional, nomeParecerista).Generate();
        }
    }
}
