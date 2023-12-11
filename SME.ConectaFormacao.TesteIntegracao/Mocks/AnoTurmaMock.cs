using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class AnoTurmaMock : BaseMock
    {
        private static Faker<AnoTurma> Gerador(bool todos)
        {
            var codigoEol = 1;
            var codigoSerieEnsino = 1000;

            var faker = new Faker<AnoTurma>();
            faker.RuleFor(dest => dest.CodigoEOL, f => codigoEol++.ToString());
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Text().Limite(70));
            faker.RuleFor(dest => dest.CodigoSerieEnsino, f => codigoSerieEnsino++);
            faker.RuleFor(dest => dest.Modalidade, f => todos ? null : f.PickRandom<Modalidade>());
            faker.RuleFor(dest => dest.Todos, todos);
            faker.RuleFor(dest => dest.Ordem, todos ? 0 : 1);
            faker.RuleFor(dest => dest.AnoLetivo, DateTimeExtension.HorarioBrasilia().Year);
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<AnoTurma> GerarAnoTurma(int quantidade)
        {
            var retorno = new List<AnoTurma>();

            retorno.AddRange(Gerador(false).Generate(quantidade));
            retorno.Add(Gerador(true).Generate());

            return retorno;
        }
    }
}
