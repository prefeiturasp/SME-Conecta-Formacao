using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock
{
    public  class AreaPromotoraSalvarMock : BaseMock
    {
        public static AreaPromotoraDTO GerarAreaPromotoraDTOValido(AreaPromotoraTipo? areaPromotoraTipo = null, string dominio = null)
        {
            var faker = new Faker<AreaPromotoraDTO>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => areaPromotoraTipo.HasValue ? areaPromotoraTipo : f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Emails, f => f.Make(2, () => new AreaPromotoraEmailDTO { Email = string.IsNullOrEmpty(dominio) ? f.Person.Email : string.Concat(f.Person.Email[..f.Person.Email.IndexOf('@')], dominio) }));
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Telefones, f => f.Make(3, () => new AreaPromotoraTelefoneDTO { Telefone = f.Phone.PhoneNumber("(##) #####-####") }));
            return faker.Generate();
        }

        public static AreaPromotoraDTO GerarAreaPromotoraComDreDTOValido(AreaPromotoraTipo? areaPromotoraTipo = null,long? dreId = null ,string dominio = null)
        {
            var faker = new Faker<AreaPromotoraDTO>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => areaPromotoraTipo.HasValue ? areaPromotoraTipo : f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Emails, f => f.Make(2, () => new AreaPromotoraEmailDTO { Email = string.IsNullOrEmpty(dominio) ? f.Person.Email : string.Concat(f.Person.Email[..f.Person.Email.IndexOf('@')], dominio) }));
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Telefones, f => f.Make(3, () => new AreaPromotoraTelefoneDTO { Telefone = f.Phone.PhoneNumber("(##) #####-####") }));
            faker.RuleFor(x => x.DreId, f => dreId ?? f.Random.Int(min:1, max:100));
            return faker.Generate();
        }
        public static IEnumerable<Dre> GerarDreValida(int quantidade = 1)
        {
            var faker = new Faker<Dre>();
            faker.RuleFor(x => x.Codigo, f => f.Random.Int(min: 1, max: 100).ToString());
            faker.RuleFor(x => x.Abreviacao, f => f.Name.FindName().Substring(10));
            faker.RuleFor(x => x.Nome, f => f.Name.FullName());
            faker.RuleFor(x => x.DataAtualizacao, DateTimeExtension.HorarioBrasilia());
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }

        public static AreaPromotoraDTO GerarAreaPromotoraDTOInvalido()
        {
            return new AreaPromotoraDTO
            {
                Emails = new AreaPromotoraEmailDTO[] { new AreaPromotoraEmailDTO { Email = new Person().FullName } }
            };
        }

        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Long(1);
        }
    }
}
