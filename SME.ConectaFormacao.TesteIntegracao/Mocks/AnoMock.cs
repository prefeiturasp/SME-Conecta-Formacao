using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class AnoMock : BaseMock
    {
        private static Faker<Ano> Gerador(Modalidade modalidade = Modalidade.Fundamental, bool todos = false)
        {
            var codigoEol = 1;
            var codigoSerieEnsino = 1000;
                
            var faker = new Faker<Ano>();
            faker.RuleFor(dest => dest.CodigoEOL, f => codigoEol++.ToString());
            faker.RuleFor(dest => dest.Descricao, f => f.Lorem.Text().Limite(70));
            faker.RuleFor(dest => dest.CodigoSerieEnsino, f=> codigoSerieEnsino++);
            faker.RuleFor(dest => dest.Modalidade, f=> modalidade);
            faker.RuleFor(dest => dest.Todos, todos);
            faker.RuleFor(dest => dest.Ordem, todos ? 0 : 1);
            faker.RuleFor(dest => dest.AnoLetivo, DateTimeExtension.HorarioBrasilia().Year);
            AuditoriaFaker(faker);
            return faker;
        }

        public static IEnumerable<Ano> GerarAno(int quantidade, bool todos = false)
        {
            var opcoesModalidades = new [] 
            { 
                Modalidade.Fundamental, 
                Modalidade.EducacaoInfantil,
                Modalidade.EJA,
                Modalidade.CIEJA,
                Modalidade.Medio,
                Modalidade.CMCT,
                Modalidade.MOVA,
                Modalidade.ETEC,
                Modalidade.CELP
            };

            var anos = new List<Ano>();
            
            foreach (var modalidade in opcoesModalidades)
                anos.AddRange(Gerador(modalidade,todos:todos).Generate(quantidade));
            
            anos.AddRange(Gerador(todos:true).Generate(1));
            
            return anos;
        }

        public static Ano GerarAno(bool todos, Modalidade modalidade = Modalidade.Fundamental)
        {
            return Gerador(modalidade,todos).Generate();
        }
    }
}
