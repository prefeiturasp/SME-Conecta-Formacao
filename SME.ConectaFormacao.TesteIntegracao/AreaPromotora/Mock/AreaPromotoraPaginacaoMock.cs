using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock
{
    public class AreaPromotoraPaginacaoMock
    {
        public static FiltrosAreaPromotoraDTO GerarFiltrosAreaPromotoraDTOVazio()
        {
            return new FiltrosAreaPromotoraDTO();
        }

        public static FiltrosAreaPromotoraDTO GerarFiltrosAreaPromotoraDTOInvalido()
        {
            var filtroInvalidoFaker = new Faker<FiltrosAreaPromotoraDTO>();
            filtroInvalidoFaker.RuleFor(x => x.Nome, f => f.Random.AlphaNumeric(14));
            filtroInvalidoFaker.RuleFor(x => x.Tipo, f => (short)f.Random.Number(0, 3));
            return filtroInvalidoFaker.Generate();
        }
    }
}
