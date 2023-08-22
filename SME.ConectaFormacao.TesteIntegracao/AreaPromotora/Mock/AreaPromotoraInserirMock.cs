using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock
{
    public static class AreaPromotoraInserirMock
    {
        public static InserirAreaPromotoraDTO InserirAreaPromotoraDTOValido { get; set; }

        public static InserirAreaPromotoraDTO InserirAreaPromotoraDTOInvalido { get; set; }

        public static void Montar()
        {
            var faker = new Faker<InserirAreaPromotoraDTO>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Email, f => f.Person.Email);
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Telefones, f => f.Make(3, () => new InserirAreaPromotoraTelefoneDTO { Telefone = f.Phone.PhoneNumber("(##) #####-####") }));
            InserirAreaPromotoraDTOValido = faker.Generate();

            InserirAreaPromotoraDTOInvalido = new InserirAreaPromotoraDTO
            {
                Email = "aaa"
            };
        }
    }
}
