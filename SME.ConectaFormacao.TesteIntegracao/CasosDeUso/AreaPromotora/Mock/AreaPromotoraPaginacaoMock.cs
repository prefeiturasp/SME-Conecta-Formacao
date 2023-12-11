using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock
{
    public class AreaPromotoraPaginacaoMock
    {
        public static AreaPromotoraFiltrosDTO GerarFiltrosAreaPromotoraDTOVazio()
        {
            return new AreaPromotoraFiltrosDTO();
        }

        public static AreaPromotoraFiltrosDTO GerarFiltrosAreaPromotoraDTOInvalido()
        {
            var filtroInvalidoFaker = new Faker<AreaPromotoraFiltrosDTO>();
            filtroInvalidoFaker.RuleFor(x => x.Nome, f => f.Random.AlphaNumeric(14));
            filtroInvalidoFaker.RuleFor(x => x.Tipo, f => (short)f.Random.Number(0, 3));
            return filtroInvalidoFaker.Generate();
        }
    }
}
