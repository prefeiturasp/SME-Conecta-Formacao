using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks
{
    public class AoRealizarInscricaoAutomaticaMock : BaseMock
    {
        public static IEnumerable<Dominio.Entidades.CargoFuncao> CargosFuncoes { get; set; }
        public static IEnumerable<Dominio.Entidades.CargoFuncaoDeparaEol> CargosFuncoesDeparaEol { get; set; }

        public static IEnumerable<CursistaServicoEol> ObterCursistasEol(int quantidade, IEnumerable<Dominio.Entidades.Dre> dres)
        {
            var cargos = CargosFuncoesDeparaEol.Where(w => w.CodigoCargoEol.HasValue).Select(w => w.CodigoCargoEol.GetValueOrDefault().ToString());
            var funcoes = CargosFuncoesDeparaEol.Where(w => w.CodigoFuncaoEol.HasValue).Select(w => w.CodigoFuncaoEol.GetValueOrDefault().ToString());

            var dresCodigo = dres.Select(s => s.Codigo);

            Faker<CursistaServicoEol> faker = new Faker<CursistaServicoEol>("pt_BR");
            faker.RuleFor(r => r.Rf, f => f.Random.Long(100000, 999999).ToString());
            faker.RuleFor(r => r.Cpf, f => f.Person.Cpf(false));
            faker.RuleFor(r => r.Nome, f => f.Person.FullName);
            faker.RuleFor(r => r.CargoCodigo, f => f.PickRandom(cargos));
            faker.RuleFor(r => r.FuncaoCodigo, f => f.PickRandom(funcoes));
            faker.RuleFor(r => r.DreCodigo, f => dres.Any() ? f.PickRandom(dresCodigo) : null);

            return faker.Generate(quantidade);
        }
    }
}
