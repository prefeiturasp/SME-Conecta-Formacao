using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks
{
    public static class AoObterGrupoMock
    {
        public static IEnumerable<GrupoDTO> Grupos { get; set; }

        public static void Montar()
        {
            var faker = new Faker<GrupoDTO>("pt_BR");
            faker.RuleFor(x => x.Id, f => Guid.NewGuid());
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            Grupos = faker.Generate(5);
        }
    }
}
