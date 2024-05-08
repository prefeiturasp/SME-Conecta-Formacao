using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaParecerMock : BaseMock
    {
        private static Faker<PropostaPareceristaConsideracao> Gerador()
        {
            var faker = new Faker<PropostaPareceristaConsideracao>("pt_BR");
            faker.RuleFor(x => x.Campo, f => (CampoParecer)f.Random.Short(1,28));
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Excluido, false);
            //TODO
            // faker.RuleFor(x => x.Situacao, SituacaoParecerista.PendenteEnvioParecerPeloParecerista);
            AuditoriaFaker(faker);
            return faker;
        }

        public static PropostaPareceristaConsideracao GerarPropostaParecer()
        {
            return Gerador().Generate();
        }
        
        public static PropostaPareceristaConsideracao GerarPropostaParecer(long propostaId, long usuarioPareceristaId, CampoParecer campoParecer, SituacaoParecerista situacaoParecerista = SituacaoParecerista.AguardandoValidacao)
        {
            var propostaParecer = GerarPropostaParecer();
            propostaParecer.PropostaPareceristaId = propostaId;
            //TODO
            // propostaParecer.UsuarioPareceristaId = usuarioPareceristaId;
            propostaParecer.Campo = campoParecer;
            // propostaParecer.Situacao = situacaoParecerista;
            return propostaParecer;
        }
        
        public static IEnumerable<PropostaPareceristaConsideracao> GerarPropostasPareceres(int quantidade = 10)
        {
            return Gerador().Generate(quantidade);
        }
    }
}
