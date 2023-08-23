using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock
{
    public static class AreaPromotoraSalvarMock
    {
        public static AreaPromotoraDTO GerarAreaPromotoraDTOValido()
        {
            var faker = new Faker<AreaPromotoraDTO>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Email, f => f.Person.Email);
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Telefones, f => f.Make(3, () => new AreaPromotoraTelefoneDTO { Telefone = f.Phone.PhoneNumber("(##) #####-####") }));
            return faker.Generate();
        }

        public static AreaPromotoraDTO GerarAreaPromotoraDTOInvalido()
        {
            return new AreaPromotoraDTO
            {
                Email = new Person().FullName
            };
        }

        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Number();
        }
    }
}
