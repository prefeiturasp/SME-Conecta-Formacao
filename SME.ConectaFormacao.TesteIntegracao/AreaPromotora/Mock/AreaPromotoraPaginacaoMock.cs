using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock
{
    public class AreaPromotoraPaginacaoMock
    {
        public static IEnumerable<Dominio.Entidades.AreaPromotora> AreasPromotoras { get; set; }

        public static FiltrosAreaPromotoraDTO FiltrosAreaPromotoraDTOValido { get; set; }

        public static FiltrosAreaPromotoraDTO FiltrosAreaPromotoraDTOVazio { get; set; }

        public static FiltrosAreaPromotoraDTO FiltrosAreaPromotoraDTOInvalido { get; set; }

        public static void Montar()
        {
            var faker = new Faker<Dominio.Entidades.AreaPromotora>("pt_BR");
            faker.RuleFor(x => x.Id, f => f.Random.Number());
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.GrupoId, Guid.NewGuid());
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            AreasPromotoras = faker.Generate(15);

            var areaPromotora = AreasPromotoras.FirstOrDefault();
            FiltrosAreaPromotoraDTOValido = new FiltrosAreaPromotoraDTO()
            {
                Nome = areaPromotora.Nome,
                Tipo = (short)areaPromotora.Tipo
            };

            FiltrosAreaPromotoraDTOVazio = new FiltrosAreaPromotoraDTO();

            FiltrosAreaPromotoraDTOInvalido = new FiltrosAreaPromotoraDTO()
            {
                Nome = FiltrosAreaPromotoraDTOValido.Nome + " invalido",
                Tipo = FiltrosAreaPromotoraDTOValido.Tipo
            };
        }
    }
}
