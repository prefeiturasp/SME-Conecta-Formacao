﻿using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaParecerMock : BaseMock
    {
        private static Faker<PropostaParecer> Gerador()
        {
            var faker = new Faker<PropostaParecer>("pt_BR");
            faker.RuleFor(x => x.Campo, f => (CampoParecer)f.Random.Short(1,28));
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker;
        }

        public static PropostaParecer GerarPropostaParecer()
        {
            return Gerador().Generate();
        }
        
        public static IEnumerable<PropostaParecer> GerarPropostasPareceres(int quantidade = 10)
        {
            return Gerador().Generate(quantidade);
        }
    }
}
